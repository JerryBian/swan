using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Blog.Extension;
using Laobian.Share.Blog.Model;
using Laobian.Share.Blog.Parser;
using Laobian.Share.Git;
using Laobian.Share.Helper;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Blog.Asset
{
    public class BlogAssetManager : IBlogAssetManager
    {
        private readonly List<BlogCategory> _allCategories;
        private readonly List<BlogPost> _allPosts;
        private readonly List<BlogTag> _allTags;

        private readonly IGitClient _gitClient;
        private readonly GitConfig _gitConfig;
        private readonly ILogger<BlogAssetManager> _logger;
        private readonly ManualResetEventSlim _manualReset;
        private readonly SemaphoreSlim _semaphore;
        private string _aboutHtml;

        public BlogAssetManager(
            IGitClient gitClient,
            ILogger<BlogAssetManager> logger)
        {
            _logger = logger;
            _allTags = new List<BlogTag>();
            _allPosts = new List<BlogPost>();
            _allCategories = new List<BlogCategory>();
            _gitClient = gitClient;
            _semaphore = new SemaphoreSlim(1, 1);
            _manualReset = new ManualResetEventSlim(true);
            _gitConfig = new GitConfig
            {
                GitHubRepositoryName = Global.Config.Blog.AssetGitHubRepoName,
                GitHubRepositoryBranch = Global.Config.Blog.AssetGitHubRepoBranch,
                GitHubRepositoryOwner = Global.Config.Blog.AssetGitHubRepoOwner,
                GitHubAccessToken = Global.Config.Blog.AssetGitHubRepoApiToken,
                GitCloneToDir = Global.Config.Blog.AssetRepoLocalDir,
                GitCommitEmail = Global.Config.Blog.AssetGitCommitEmail,
                GitCommitUser = Global.Config.Blog.AssetGitCommitUser
            };
        }

        #region Public Interfaces

        public async Task<bool> PullFromGitHubAsync()
        {
            try
            {
                await _gitClient.CloneToLocalAsync(_gitConfig);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Pull assets from GitHub failed.");
                return false;
            }
        }

        public async Task<bool> ParseAssetsToObjectsAsync()
        {
            try
            {
                await _semaphore.WaitAsync();
                var postTask = ReloadLocalMemoryPostAsync();
                var categoryTask = ReloadLocalMemoryCategoryAsync();
                var tagTask = ReloadLocalMemoryTagAsync();
                var aboutTask = ReloadLocalMemoryAboutAsync();
                await Task.WhenAll(postTask, categoryTask, tagTask, aboutTask);

                var posts = await postTask;
                var categories = await categoryTask;
                var tags = await tagTask;
                var aboutHtml = await aboutTask;

                try
                {
                    _manualReset.Reset();
                    RefreshMemoryAsset(posts, categories, tags, aboutHtml);
                    return true;
                }
                finally
                {
                    _manualReset.Set();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Parse assets to objects failed.");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> SerializeAssetsToFilesAsync()
        {
            try
            {
                var postMetadata = new List<BlogPostMetadata>();
                foreach (var blogPost in _allPosts)
                {
                    postMetadata.Add(blogPost.Metadata);
                }

                var postMetadataPath =
                    Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.PostMetadataPath);
                var metadataParseResult = BlogAssetParser.ToJson(postMetadata.OrderByDescending(p => p.CreateTime));
                LogParseResultMessages(metadataParseResult);

                if (metadataParseResult.Success)
                {
                    if (!Directory.Exists(Path.GetDirectoryName(postMetadataPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(postMetadataPath));
                    }

                    await File.WriteAllTextAsync(postMetadataPath, metadataParseResult.Instance, Encoding.UTF8);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Serialize assets to files failed.");
                return false;
            }
        }

        public async Task<bool> PushToGitHubAsync(string message)
        {
            try
            {
                await _gitClient.CommitAsync(Global.Config.Blog.AssetRepoLocalDir, message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Push to GitHub failed.");
                return false;
            }
        }

        public bool MergePosts(List<BlogPost> oldPosts)
        {
            try
            {
                foreach (var blogPost in _allPosts)
                {
                    var oldPost = oldPosts.FirstOrDefault(p => CompareHelper.IgnoreCase(p.Link, blogPost.Link));
                    if (oldPost != null)
                    {
                        blogPost.AccessCount = Math.Max(oldPost.AccessCount, blogPost.AccessCount);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Merge assets failed.");
                return false;
            }
        }

        public bool UpdatePosts(IEnumerable<string> postLinks)
        {
            try
            {
                foreach (var postLink in postLinks)
                {
                    var post = _allPosts.FirstOrDefault(p =>
                        CompareHelper.IgnoreCase(p.Link, Path.GetFileNameWithoutExtension(postLink)));
                    if (post != null)
                    {
                        post.Metadata.LastUpdateTime = DateTime.Now;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Update posts failed.");
                return false;
            }
        }

        public List<BlogPost> GetAllPosts()
        {
            _manualReset.Wait();
            return _allPosts;
        }

        public List<BlogCategory> GetAllCategories()
        {
            _manualReset.Wait();
            return _allCategories;
        }

        public List<BlogTag> GetAllTags()
        {
            _manualReset.Wait();
            return _allTags;
        }

        public string GetAboutHtml()
        {
            _manualReset.Wait();
            return _aboutHtml;
        }

        #endregion


        #region Private Methods

        private void RefreshMemoryAsset(
            List<BlogPost> posts,
            List<BlogCategory> categories,
            List<BlogTag> tags,
            string aboutHtml)
        {
            _allPosts.Clear();
            _allPosts.AddRange(posts);

            _allCategories.Clear();
            _allCategories.AddRange(categories);

            _allTags.Clear();
            _allTags.AddRange(tags);

            _aboutHtml = aboutHtml;

            var postsPublishTime = new ConcurrentBag<DateTime>();
            Parallel.ForEach(_allPosts, blogPost =>
            {
                blogPost.Resolve(_allCategories, _allTags);
                var rawPublishTime = blogPost.GetRawPublishTime();
                if (rawPublishTime.HasValue && rawPublishTime != default(DateTime))
                {
                    postsPublishTime.Add(rawPublishTime.Value);
                }
            });

            BlogState.PostsPublishTime = postsPublishTime.OrderBy(p => p);
            Parallel.ForEach(_allCategories, blogCategory => { blogCategory.Resolve(_allPosts); });
            Parallel.ForEach(_allTags, blogTag => { blogTag.Resolve(_allPosts); });
            BlogState.AssetLastUpdate = DateTime.Now;
        }

        private async Task<List<BlogPost>> ReloadLocalMemoryPostAsync()
        {
            var result = new ConcurrentBag<BlogPost>();
            var postLocalPath = Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.PostGitPath);
            if (!Directory.Exists(postLocalPath))
            {
                _logger.LogWarning($"No post folder found under \"{postLocalPath}\".");
                return result.ToList();
            }

            var postMetadata = new List<BlogPostMetadata>();
            var metadataLocalPath =
                Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.PostMetadataPath);
            if (File.Exists(metadataLocalPath))
            {
                var metadataText = await File.ReadAllTextAsync(metadataLocalPath);
                var metadataParseResult = BlogAssetParser.ParseJson<List<BlogPostMetadata>>(metadataText);
                LogParseResultMessages(metadataParseResult);

                if (metadataParseResult.Success)
                {
                    postMetadata = metadataParseResult.Instance;
                }
                else
                {
                    throw new Exception("Post metadata parse failed, please check errors.");
                }
            }

            Parallel.ForEach(Directory.EnumerateFiles(postLocalPath, $"*{Global.Config.Common.MarkdownExtension}"),
                file =>
                {
                    try
                    {
                        var text = File.ReadAllText(file);
                        var parseResult = BlogAssetParser.ToText(text);
                        LogParseResultMessages(parseResult);

                        if (parseResult.Success)
                        {
                            var post = new BlogPost
                            {
                                Link = Path.GetFileNameWithoutExtension(file),
                                GitPath = file.Substring(
                                    file.IndexOf(Global.Config.Blog.AssetRepoLocalDir,
                                        StringComparison.CurrentCulture) +
                                    Global.Config.Blog.AssetRepoLocalDir.Length + 1),
                                LocalPath = file,
                                ContentMarkdown = parseResult.Instance
                            };

                            var metadata =
                                postMetadata.FirstOrDefault(m => CompareHelper.IgnoreCase(m.Link, post.Link));
                            if (metadata != null)
                            {
                                post.Metadata = metadata;
                            }
                            else
                            {
                                post.Metadata.Link = post.Link;
                            }

                            result.Add(post);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            $"Parse post throw exception: {file}.{Environment.NewLine}{ex}{Environment.NewLine}");
                    }
                });

            return result.ToList();
        }

        private async Task<List<BlogCategory>> ReloadLocalMemoryCategoryAsync()
        {
            var result = new List<BlogCategory>();
            var categoryLocalPath =
                Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.CategoryGitPath);
            if (!File.Exists(categoryLocalPath))
            {
                _logger.LogWarning($"No category asset found under \"{categoryLocalPath}\".");
                return result;
            }

            var text = await File.ReadAllTextAsync(categoryLocalPath);
            var parseResult = await BlogAssetParser.ParseColonSeparatedTextAsync(text);
            LogParseResultMessages(parseResult);

            if (parseResult.Success)
            {
                foreach (var item in parseResult.Instance)
                {
                    result.Add(new BlogCategory
                    {
                        Name = item.Key,
                        Link = item.Value
                    });
                }
            }

            return result;
        }

        private async Task<List<BlogTag>> ReloadLocalMemoryTagAsync()
        {
            var result = new List<BlogTag>();
            var tagLocalPath = Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.TagGitPath);
            if (!File.Exists(tagLocalPath))
            {
                _logger.LogWarning($"No tag asset found under \"{tagLocalPath}\".");
                return result;
            }

            var text = await File.ReadAllTextAsync(tagLocalPath);
            var parseResult = await BlogAssetParser.ParseColonSeparatedTextAsync(text);
            LogParseResultMessages(parseResult);

            if (parseResult.Success)
            {
                foreach (var item in parseResult.Instance)
                {
                    result.Add(new BlogTag
                    {
                        Name = item.Key,
                        Link = item.Value
                    });
                }
            }

            return result;
        }

        private async Task<string> ReloadLocalMemoryAboutAsync()
        {
            var result = string.Empty;
            var aboutLocalPath = Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.AboutGitPath);
            if (!File.Exists(aboutLocalPath))
            {
                _logger.LogWarning($"No about asset found under \"{aboutLocalPath}\".");
                return result;
            }

            var md = await File.ReadAllTextAsync(aboutLocalPath);
            var parseResult = BlogAssetParser.ToText(md);
            LogParseResultMessages(parseResult);

            if (parseResult.Success)
            {
                result = MarkdownHelper.ToHtml(md);
            }

            return result;
        }

        private void LogParseResultMessages<T>(BlogAssetParseResult<T> result)
        {
            var message = result.AggregateMessages();
            if (!string.IsNullOrEmpty(message))
            {
                _logger.LogError(message);
            }
        }

        #endregion
    }
}