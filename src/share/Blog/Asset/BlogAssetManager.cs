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

        private readonly IGitClient _gitClient;
        private readonly GitConfig _gitConfig;
        private readonly ManualResetEventSlim _manualReset;
        private readonly SemaphoreSlim _semaphore;
        private readonly ILogger<BlogAssetManager> _logger;

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

        public async Task RemoteGitToLocalFileAsync()
        {
            await _gitClient.CloneToLocalAsync(_gitConfig);
        }

        public async Task<bool> LocalFileToLocalMemoryAsync()
        {
            try
            {
                await _semaphore.WaitAsync();
                var posts = await ReloadLocalMemoryPostAsync();
                var categories = await ReloadLocalMemoryCategoryAsync();
                var tags = await ReloadLocalMemoryTagAsync();
                var aboutHtml = await ReloadLocalMemoryAboutAsync();

                try
                {
                    _manualReset.Reset();
                    RefreshMemoryAsset(posts, categories, tags, aboutHtml);
                }
                finally
                {
                    _manualReset.Set();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Local file to memory failed.");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }

            return true;
        }

        public async Task LocalMemoryToLocalFileAsync()
        {
            var postMetadata = new List<BlogPostMetadata>();

            foreach (var blogPost in _allPosts)
            {
                var contentParseResult = BlogAssetParser.ToText(blogPost.ContentMarkdown);
                if (contentParseResult.Success)
                {
                    await File.WriteAllTextAsync(blogPost.LocalPath, contentParseResult.Instance, Encoding.UTF8);
                }

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
        }

        public async Task LocalFileToRemoteGitAsync()
        {
            await _gitClient.CommitAsync(Global.Config.Blog.AssetRepoLocalDir, "Update assets");
        }

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

            foreach (var blogPost in _allPosts)
            {
                blogPost.Resolve(_allCategories, _allTags);
            }

            foreach (var blogCategory in _allCategories)
            {
                blogCategory.Resolve(_allPosts);
            }

            foreach (var blogTag in _allTags)
            {
                blogTag.Resolve(_allPosts);
            }
        }

        private async Task<List<BlogPost>> ReloadLocalMemoryPostAsync()
        {
            var result = new List<BlogPost>();
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
                if (metadataParseResult.Success)
                {
                    postMetadata = metadataParseResult.Instance;
                }
            }


            foreach (var file in Directory.EnumerateFiles(postLocalPath, $"*{Global.Config.Common.MarkdownExtension}"))
            {
                try
                {
                    var text = await File.ReadAllTextAsync(file);
                    var parseResult = BlogAssetParser.ToText(text);
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
            if (parseResult.Success)
            {
                result = MarkdownHelper.ToHtml(md);
            }

            return result;
        }
    }
}