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

namespace Laobian.Share.Blog.Asset
{
    public class BlogAssetManager : IBlogAssetManager
    {
        private readonly List<BlogCategory> _allCategories;
        private readonly List<BlogPost> _allPosts;
        private readonly List<BlogTag> _allTags;

        private readonly IGitClient _gitClient;
        private readonly GitConfig _gitConfig;
        private readonly ManualResetEventSlim _manualReset;
        private readonly SemaphoreSlim _semaphore;

        public BlogAssetManager(
            IGitClient gitClient)
        {
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

        public async Task PullFromGitHubAsync()
        {
            await _gitClient.CloneToLocalAsync(_gitConfig);
        }

        public async Task<string> ParseAssetsToObjectsAsync()
        {
            try
            {
                await _semaphore.WaitAsync();
                var postTask = ReloadLocalMemoryPostAsync();
                var categoryTask = ReloadLocalMemoryCategoryAsync();
                var tagTask = ReloadLocalMemoryTagAsync();
                await Task.WhenAll(postTask, categoryTask, tagTask);

                var posts = await postTask;
                var categories = await categoryTask;
                var tags = await tagTask;

                try
                {
                    _manualReset.Reset();
                    RefreshMemoryAsset(posts.Instance, categories.Instance, tags.Instance);
                    var result = new StringBuilder();
                    var agg = posts.AggregateMessages();
                    if (!string.IsNullOrEmpty(agg))
                    {
                        result.AppendLine(agg);
                    }

                    agg = categories.AggregateMessages();
                    if (!string.IsNullOrEmpty(agg))
                    {
                        result.AppendLine(agg);
                    }

                    agg = tags.AggregateMessages();
                    if (!string.IsNullOrEmpty(agg))
                    {
                        result.AppendLine(agg);
                    }

                    return result.ToString();
                }
                finally
                {
                    _manualReset.Set();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task SerializeAssetsToFilesAsync()
        {
            var postMetadata = new List<BlogPostMetadata>();
            foreach (var blogPost in _allPosts)
            {
                postMetadata.Add(blogPost.Metadata);
            }

            var postMetadataPath =
                Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.PostMetadataPath);
            var metadataParseResult = BlogAssetParser.ToJson(postMetadata.OrderByDescending(p => p.CreateTime));

            if (metadataParseResult.Success)
            {
                if (!Directory.Exists(Path.GetDirectoryName(postMetadataPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(postMetadataPath));
                }

                await File.WriteAllTextAsync(postMetadataPath, metadataParseResult.Instance, Encoding.UTF8);
            }
            else
            {
                throw new Exception($"SerializeAssetsToFiles failed. {metadataParseResult.AggregateMessages()}");
            }
        }

        public async Task PushToGitHubAsync(string message)
        {
            await _gitClient.CommitAsync(Global.Config.Blog.AssetRepoLocalDir, message);
        }

        public void MergePosts(List<BlogPost> oldPosts)
        {
            foreach (var blogPost in _allPosts)
            {
                var oldPost = oldPosts.FirstOrDefault(p => CompareHelper.IgnoreCase(p.Link, blogPost.Link));
                if (oldPost != null)
                {
                    blogPost.AccessCount = Math.Max(oldPost.AccessCount, blogPost.AccessCount);
                }
            }
        }

        public void UpdatePosts(IEnumerable<string> postLinks)
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

        #endregion


        #region Private Methods

        private void RefreshMemoryAsset(
            List<BlogPost> posts,
            List<BlogCategory> categories,
            List<BlogTag> tags)
        {
            _allPosts.Clear();
            _allPosts.AddRange(posts);

            _allCategories.Clear();
            _allCategories.AddRange(categories);

            _allTags.Clear();
            _allTags.AddRange(tags);

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

        private async Task<BlogAssetLoadResult<List<BlogPost>>> ReloadLocalMemoryPostAsync()
        {
            var result = new BlogAssetLoadResult<List<BlogPost>> { Description = "Load Posts" };
            var posts = new ConcurrentBag<BlogPost>();
            
            var postLocalPath = Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.PostGitPath);
            if (!Directory.Exists(postLocalPath))
            {
                result.Instance = posts.ToList();
                result.Warnings.Add($"No post folder found under \"{postLocalPath}\".");
                return result;
            }

            var postMetadata = new List<BlogPostMetadata>();
            var metadataLocalPath =
                Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.PostMetadataPath);
            if (File.Exists(metadataLocalPath))
            {
                var metadataText = await File.ReadAllTextAsync(metadataLocalPath);
                var metadataParseResult = BlogAssetParser.ParseJson<List<BlogPostMetadata>>(metadataText);

                if (metadataParseResult.Success)
                {
                    postMetadata = metadataParseResult.Instance;
                    result.Warnings.AddRange(metadataParseResult.WarningMessages);
                    result.Errors.AddRange(metadataParseResult.ErrorMessages);
                }
                else
                {
                    throw new Exception($"Post metadata parse failed, please check errors.{metadataParseResult.AggregateMessages()}");
                }
            }

            Parallel.ForEach(Directory.EnumerateFiles(postLocalPath, $"*{Global.Config.Common.MarkdownExtension}"),
                file =>
                {
                    var text = File.ReadAllText(file);
                    var parseResult = BlogAssetParser.ToText(text);

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

                        posts.Add(post);
                        result.Warnings.AddRange(parseResult.WarningMessages);
                        result.Errors.AddRange(parseResult.ErrorMessages);
                    }
                    else
                    {
                        throw new Exception($"Post({postLocalPath}) parse failed, please check errors.{parseResult.AggregateMessages()}");
                    }
                });

            result.Instance = posts.ToList();
            return result;
        }

        private async Task<BlogAssetLoadResult<List<BlogCategory>>> ReloadLocalMemoryCategoryAsync()
        {
            var result = new BlogAssetLoadResult<List<BlogCategory>> { Description = "Load Categories" };
            var categories = new List<BlogCategory>();
            result.Instance = categories;
            var categoryLocalPath =
                Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.CategoryGitPath);
            if (!File.Exists(categoryLocalPath))
            {
                result.Warnings.Add($"No category asset found under \"{categoryLocalPath}\".");
                return result;
            }

            var text = await File.ReadAllTextAsync(categoryLocalPath);
            var parseResult = await BlogAssetParser.ParseColonSeparatedTextAsync(text);

            if (parseResult.Success)
            {
                foreach (var item in parseResult.Instance)
                {
                    categories.Add(new BlogCategory
                    {
                        Name = item.Key,
                        Link = item.Value
                    });
                }
            }
            else
            {
                throw new Exception($"Category parse failed, please check errors.{parseResult.AggregateMessages()}");
            }

            return result;
        }

        private async Task<BlogAssetLoadResult<List<BlogTag>>> ReloadLocalMemoryTagAsync()
        {
            var result = new BlogAssetLoadResult<List<BlogTag>> { Description = "Load Tags" };
            var tags = new List<BlogTag>();
            result.Instance = tags;
            var tagLocalPath = Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.TagGitPath);
            if (!File.Exists(tagLocalPath))
            {
                result.Warnings.Add($"No tag asset found under \"{tagLocalPath}\".");
                return result;
            }

            var text = await File.ReadAllTextAsync(tagLocalPath);
            var parseResult = await BlogAssetParser.ParseColonSeparatedTextAsync(text);

            if (parseResult.Success)
            {
                foreach (var item in parseResult.Instance)
                {
                    tags.Add(new BlogTag
                    {
                        Name = item.Key,
                        Link = item.Value
                    });
                }
            }
            else
            {
                throw new Exception($"Tag parse failed, please check errors.{parseResult.AggregateMessages()}");
            }

            return result;
        }

        #endregion
    }
}