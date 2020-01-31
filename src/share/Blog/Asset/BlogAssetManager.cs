using System;
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
<<<<<<< HEAD
        private string _aboutHtml;

=======

        private readonly BlogCategoryParser _categoryParser;
>>>>>>> master
        private readonly IGitClient _gitClient;
        private readonly GitConfig _gitConfig;
        private readonly ManualResetEventSlim _manualReset;
        private readonly SemaphoreSlim _semaphore;
<<<<<<< HEAD
        private readonly ILogger<BlogAssetManager> _logger;

        public BlogAssetManager(
            IGitClient gitClient,
            ILogger<BlogAssetManager> logger)
=======
        private readonly BlogTagParser _tagParser;
        private readonly BlogPostVisitParser _postVisitParser;

        private string _aboutHtml;
        private BlogPostAccess _allPostAccess;

        public BlogAssetManager(IGitClient gitClient)
>>>>>>> master
        {
            _logger = logger;
            _allTags = new List<BlogTag>();
            _allPosts = new List<BlogPost>();
            _allCategories = new List<BlogCategory>();
            _allPostAccess = new BlogPostAccess();
            _gitClient = gitClient;
<<<<<<< HEAD
=======
            _postParser = new BlogPostParser();
            _categoryParser = new BlogCategoryParser();
            _tagParser = new BlogTagParser();
            _postParser = new BlogPostParser();
            _postVisitParser = new BlogPostVisitParser();
>>>>>>> master
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
<<<<<<< HEAD
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
=======
            _manualReset.Wait();
            return _allTags;
        }

        public string GetAboutHtml()
        {
            _manualReset.Wait();
            return _aboutHtml;
        }

        public BlogPostAccess GetPostVisit()
        {
            _manualReset.Wait();
            return _allPostAccess;
        }

        public async Task RemoteGitToLocalFileAsync()
        {
            await _gitClient.CloneToLocalAsync(_gitConfig);
        }

        public async Task UpdateRemoteGitTemplatePostAsync()
        {
            var templatePost = new BlogPost();
            templatePost.Raw.IsDraft = true;
            templatePost.Raw.PublishTime = new DateTime(2017, 08, 31, 16, 06, 05);
            templatePost.Raw.Category = new List<string> { "分类名称1", "分类名称2" };
            templatePost.Raw.Tag = new List<string> { "标签名称1", "标签名称2" };
            templatePost.Raw.CreateTime = new DateTime(2012, 10, 16, 02, 21, 01);
            templatePost.Raw.ContainsMath = false;
            templatePost.Raw.IsTopping = false;
            templatePost.Raw.LastUpdateTime = new DateTime(2019, 01, 01, 08, 08, 08);
            templatePost.Raw.Link = "Your-Post-Unique-Link";
            templatePost.Raw.Title = "Your Post Title";
            templatePost.Raw.Markdown = "Your Post Content in Markdown.";

            var text = await _postParser.ToTextAsync(templatePost);
            var templatePostLocalPath =
                Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.TemplatePostGitPath);
            await File.WriteAllTextAsync(templatePostLocalPath, text, Encoding.UTF8);
            await _gitClient.CommitAsync(Global.Config.Blog.AssetRepoLocalDir, "Update template post");
>>>>>>> master
        }

        public async Task<bool> ParseAssetsToObjectsAsync()
        {
            try
            {
                await _semaphore.WaitAsync();
<<<<<<< HEAD
                var posts = await ReloadLocalMemoryPostAsync();
                var categories = await ReloadLocalMemoryCategoryAsync();
                var tags = await ReloadLocalMemoryTagAsync();
                var aboutHtml = await ReloadLocalMemoryAboutAsync();

                try
                {
                    _manualReset.Reset();
                    RefreshMemoryAsset(posts, categories, tags, aboutHtml);
                    return true;
=======
                var postReloadResult = await ReloadLocalMemoryPostAsync();
                var categoryReloadResult = await ReloadLocalMemoryCategoryAsync();
                var tagReloadResult = await ReloadLocalMemoryTagAsync();
                var aboutReloadResult = await ReloadLocalMemoryAboutAsync();
                var postVisitReloadResult = await ReloadLocalMemoryPostVisitAsync();

                var warning = GetWarning(postReloadResult, categoryReloadResult, tagReloadResult, aboutReloadResult, postVisitReloadResult);
                var error = GetError(postReloadResult, categoryReloadResult, tagReloadResult, aboutReloadResult, postVisitReloadResult);
                var result = new BlogAssetReloadResult<object>
                {
                    Warning = warning,
                    Error = error
                };

                if (postReloadResult.Success &&
                    categoryReloadResult.Success &&
                    tagReloadResult.Success &&
                    aboutReloadResult.Success &&
                    postVisitReloadResult.Success)
                {
                    try
                    {
                        _manualReset.Reset();
                        RefreshMemoryAsset(postReloadResult, categoryReloadResult, tagReloadResult, aboutReloadResult, postVisitReloadResult);
                    }
                    finally
                    {
                        _manualReset.Set();
                    }
>>>>>>> master
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
<<<<<<< HEAD
                var postMetadata = new List<BlogPostMetadata>();
                foreach (var blogPost in _allPosts)
                {
                    postMetadata.Add(blogPost.Metadata);
                }
=======
                var text = await _postParser.ToTextAsync(blogPost);
                await File.WriteAllTextAsync(blogPost.LocalPath, text, Encoding.UTF8);
            }

            var postVisitText = await _postVisitParser.ToTextAsync(_allPostAccess);
            await File.WriteAllTextAsync(
                Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.PostAccessGitPath),
                postVisitText,
                Encoding.UTF8);
        }
>>>>>>> master

                var postMetadataPath =
                    Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.PostMetadataPath);
                var metadataParseResult = BlogAssetParser.ToJson(postMetadata.OrderByDescending(p => p.CreateTime));
                LogParseResultMessages(metadataParseResult);

<<<<<<< HEAD
                if (metadataParseResult.Success)
                {
                    if (!Directory.Exists(Path.GetDirectoryName(postMetadataPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(postMetadataPath));
                    }

                    await File.WriteAllTextAsync(postMetadataPath, metadataParseResult.Instance, Encoding.UTF8);
                }
=======
        private static string GetError(
            BlogAssetReloadResult<List<BlogPost>> postReloadResult,
            BlogAssetReloadResult<List<BlogCategory>> categoryReloadResult,
            BlogAssetReloadResult<List<BlogTag>> tagReloadResult,
            BlogAssetReloadResult<string> aboutReloadResult,
            BlogAssetReloadResult<BlogPostAccess> postVisitReloadResult)
        {
            var errors = new List<string>();
            if (!string.IsNullOrEmpty(postReloadResult.Error))
            {
                errors.Add(postReloadResult.Error);
            }
>>>>>>> master

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
<<<<<<< HEAD
        }

        public bool MergePosts(List<BlogPost> oldPosts)
=======

            if (!string.IsNullOrEmpty(postVisitReloadResult.Error))
            {
                errors.Add(postVisitReloadResult.Error);
            }

            var error = errors.Any() ? string.Join(Environment.NewLine, errors) : string.Empty;
            return error;
        }

        private static string GetWarning(
            BlogAssetReloadResult<List<BlogPost>> postReloadResult,
            BlogAssetReloadResult<List<BlogCategory>> categoryReloadResult,
            BlogAssetReloadResult<List<BlogTag>> tagReloadResult,
            BlogAssetReloadResult<string> aboutReloadResult,
            BlogAssetReloadResult<BlogPostAccess> postVisitReloadResult)
>>>>>>> master
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
                    var post = _allPosts.FirstOrDefault(p => CompareHelper.IgnoreCase(p.Link, postLink));
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

<<<<<<< HEAD
        public List<BlogTag> GetAllTags()
        {
            _manualReset.Wait();
            return _allTags;
=======
            if (!string.IsNullOrEmpty(postVisitReloadResult.Warning))
            {
                warnings.Add(postVisitReloadResult.Warning);
            }

            var warning = warnings.Any() ? string.Join(Environment.NewLine, warnings) : string.Empty;
            return warning;
>>>>>>> master
        }

        public string GetAboutHtml()
        {
            _manualReset.Wait();
            return _aboutHtml;
        }

        #endregion


        #region Private Methods

        private void RefreshMemoryAsset(
<<<<<<< HEAD
            List<BlogPost> posts,
            List<BlogCategory> categories,
            List<BlogTag> tags,
            string aboutHtml)
=======
            BlogAssetReloadResult<List<BlogPost>> postReloadResult,
            BlogAssetReloadResult<List<BlogCategory>> categoryReloadResult,
            BlogAssetReloadResult<List<BlogTag>> tagReloadResult,
            BlogAssetReloadResult<string> aboutReloadResult,
            BlogAssetReloadResult<BlogPostAccess> postVisitReloadResult)
>>>>>>> master
        {
            _allPosts.Clear();
            _allPosts.AddRange(posts);

            _allCategories.Clear();
            _allCategories.AddRange(categories);

            _allTags.Clear();
            _allTags.AddRange(tags);

<<<<<<< HEAD
            _aboutHtml = aboutHtml;
=======
            _aboutHtml = aboutReloadResult.Result;
            _allPostAccess = postVisitReloadResult.Result;
>>>>>>> master

            var postsPublishTime = new List<DateTime>();
            foreach (var blogPost in _allPosts)
            {
<<<<<<< HEAD
                blogPost.Resolve(_allCategories, _allTags);
                var rawPublishTime = blogPost.GetRawPublishTime();
                if (rawPublishTime.HasValue && rawPublishTime != default(DateTime))
                {
                    postsPublishTime.Add(rawPublishTime.Value);
                }
=======
                blogPost.Resolve(_allCategories, _allTags, _allPostAccess);
>>>>>>> master
            }

            BlogState.PostsPublishTime = postsPublishTime.OrderBy(p => p);

            foreach (var blogCategory in _allCategories)
            {
                blogCategory.Resolve(_allPosts);
            }

            foreach (var blogTag in _allTags)
            {
                blogTag.Resolve(_allPosts);
            }

            BlogState.AssetLastUpdate = DateTime.Now;
        }

        private async Task<List<BlogPost>> ReloadLocalMemoryPostAsync()
        {
<<<<<<< HEAD
            var result = new List<BlogPost>();
=======
            var result = new BlogAssetReloadResult<List<BlogPost>> { Result = new List<BlogPost>() };
>>>>>>> master
            var postLocalPath = Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.PostGitPath);
            if (!Directory.Exists(postLocalPath))
            {
                _logger.LogWarning($"No post folder found under \"{postLocalPath}\".");
                return result;
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


            foreach (var file in Directory.EnumerateFiles(postLocalPath, $"*{Global.Config.Common.MarkdownExtension}"))
            {
                try
                {
                    var text = await File.ReadAllTextAsync(file);
                    var parseResult = BlogAssetParser.ToText(text);
                    LogParseResultMessages(parseResult);

                    if (parseResult.Success)
                    {
                        var post = new BlogPost
                        {
                            Link = Path.GetFileNameWithoutExtension(file),
                            GitPath = file.Substring(
                                file.IndexOf(Global.Config.Blog.AssetRepoLocalDir, StringComparison.CurrentCulture) +
                                Global.Config.Blog.AssetRepoLocalDir.Length + 1),
                            LocalPath = file,
                            ContentMarkdown = parseResult.Instance
                        };

                        var metadata = postMetadata.FirstOrDefault(m => CompareHelper.IgnoreCase(m.Link, post.Link));
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
            }

            return result;
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

<<<<<<< HEAD
        private void LogParseResultMessages<T>(BlogAssetParseResult<T> result)
        {
            var message = result.AggregateMessages();
            if (!string.IsNullOrEmpty(message))
            {
                _logger.LogError(message);
            }
        }

        #endregion
=======
        private async Task<BlogAssetReloadResult<BlogPostAccess>> ReloadLocalMemoryPostVisitAsync()
        {
            var result = new BlogAssetReloadResult<BlogPostAccess> { Result = new BlogPostAccess() };
            var postVisitLocalPath = Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.PostAccessGitPath);
            if (!File.Exists(postVisitLocalPath))
            {
                result.Success = true;
                result.Warning = $"No post visit asset found under \"{postVisitLocalPath}\".";
                return result;
            }

            var text = await File.ReadAllTextAsync(postVisitLocalPath);
            var parseResult = await _postVisitParser.FromTextAsync(text);
            if (parseResult.WarningMessages.Any())
            {
                result.Warning =
                    $"Blog post visit parse warnings: {Environment.NewLine}{string.Join(Environment.NewLine, parseResult.WarningMessages)}";
            }

            if (parseResult.ErrorMessages.Any())
            {
                result.Warning =
                    $"Blog post visit parse errors: {Environment.NewLine}{string.Join(Environment.NewLine, parseResult.ErrorMessages)}";
            }

            result.Success = parseResult.Success;
            result.Result = parseResult.Instance;
            return result;
        }
>>>>>>> master
    }
}