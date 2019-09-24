using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine.Model;
using Laobian.Share.BlogEngine.Parser;
using Laobian.Share.Config;
using Laobian.Share.Infrastructure.Cache;
using Laobian.Share.Infrastructure.GitHub;
using Microsoft.Extensions.Options;

namespace Laobian.Share.BlogEngine
{
    public class BlogService : IBlogService
    {
        private readonly GitConfig _gitConfig;
        private readonly AppConfig _appConfig;
        private readonly BlogTagParser _tagParser;
        private readonly BlogPostParser _postParser;
        private readonly IGitHubClient _gitHubClient;
        private readonly BlogCategoryParser _categoryParser;
        private readonly IMemoryCacheClient _memoryCacheClient;

        public BlogService(
            IOptions<AppConfig> appConfig,
            IGitHubClient gitHubClient,
            IMemoryCacheClient memoryCacheClient)
        {
            _appConfig = appConfig.Value;
            _gitHubClient = gitHubClient;
            _memoryCacheClient = memoryCacheClient;

            _tagParser = new BlogTagParser();
            _postParser = new BlogPostParser();
            _categoryParser = new BlogCategoryParser();
            _gitConfig = new GitConfig
            {
                GitHubRepositoryName = _appConfig.AssetGitHubRepoName,
                GitHubRepositoryBranch = _appConfig.AssetGitHubRepoBranch,
                GitHubRepositoryOwner = _appConfig.AssetGitHubRepoOwner,
                GitHubAccessToken = _appConfig.AssetGitHubRepoApiToken,
                GitCloneToDir = _appConfig.AssetRepoLocalDir
            };
        }

        #region Public Interface

        public BlogPost GetPost(int year, int month, string link)
        {
            var posts = GetPosts();
            var post = posts.FirstOrDefault(p =>
                string.Equals(p.FullUrl, BlogPost.GetFullUrl(year, month, link), StringComparison.OrdinalIgnoreCase));
            post?.AddVisit();

            return post;
        }

        public List<BlogPost> GetPosts()
        {
            if (!_memoryCacheClient.TryGet<List<BlogPost>>(BlogConstant.PostMemCacheKey, out var posts))
            {
                posts = new List<BlogPost>();
            }

            return posts;
        }

        public List<BlogCategory> GetCategories()
        {
            if (!_memoryCacheClient.TryGet<List<BlogCategory>>(BlogConstant.CategoryMemCacheKey, out var categories))
            {
                categories = new List<BlogCategory>();
            }

            return categories;
        }

        public List<BlogTag> GetTags()
        {
            if (!_memoryCacheClient.TryGet<List<BlogTag>>(BlogConstant.TagMemCacheKey, out var tags))
            {
                tags = new List<BlogTag>();
            }

            return tags;
        }

        public async Task UpdateCloudAssetsAsync()
        {
            await UpdateCloudPostsAsync();
        }

        public async Task UpdateLocalAssetsAsync()
        {
            await _gitHubClient.CloneAsync(_gitConfig);

            var tasks = new List<Task>
            {
                UpdateLocalCategoriesAsync(),
                UpdateLocalPostsAsync(),
                UpdateLocalTagsAsync()
            };
            await Task.WhenAll(tasks);
        }

        #endregion

        #region Blog Post

        private async Task UpdateCloudPostsAsync()
        {
            var posts = GetPosts();
            foreach (var post in posts)
            {
                await _gitHubClient.CommitPlanTextAsync(
                    _gitConfig,
                    post.GitHubPath,
                    await _postParser.ToTextAsync(post),
                    GitHubMessageProvider.GetPostCommitMessage());
            }
        }

        private async Task UpdateLocalPostsAsync()
        {
            var memPosts = GetPosts();
            var filePosts = await GetPostsFromFileAsync();
            var posts = new List<BlogPost>();
            foreach (var filePost in filePosts)
            {
                var memPost = memPosts.FirstOrDefault(p =>
                    string.Equals(p.Link, filePost.Link, StringComparison.OrdinalIgnoreCase));
                if (memPost != null)
                {
                    filePost.Visits = Math.Max(filePost.Visits, memPost.Visits);
                }

                posts.Add(filePost);
            }

            _memoryCacheClient.Set(BlogConstant.PostMemCacheKey, posts, TimeSpan.FromDays(1));
        }

        private async Task<List<BlogPost>> GetPostsFromFileAsync()
        {
            var result = new List<BlogPost>();
            var postDirPath = Path.Combine(_appConfig.AssetRepoLocalDir, BlogConstant.PostGitHubPath);
            if (!Directory.Exists(postDirPath))
            {
                return result;
            }

            foreach (var postItem in Directory.EnumerateFiles(
                postDirPath,
                $"*{BlogConstant.PostMarkdownExtension}",
                SearchOption.TopDirectoryOnly))
            {
                var post = await _postParser.FromTextAsync(await File.ReadAllTextAsync(postItem), Path.GetFileNameWithoutExtension(postItem));
                result.Add(post);
            }

            return result;
        }

        #endregion

        #region Blog Category

        private async Task UpdateLocalCategoriesAsync()
        {
            var categories = await GetCategoriesFromFileAsync();
            _memoryCacheClient.Set(BlogConstant.CategoryMemCacheKey, categories, TimeSpan.FromDays(1));
        }

        private async Task<List<BlogCategory>> GetCategoriesFromFileAsync()
        {
            var result = new List<BlogCategory>();
            var categoryPath = Path.Combine(_appConfig.AssetRepoLocalDir, BlogConstant.CategoryGitHubPath);
            if (!File.Exists(categoryPath))
            {
                return result;
            }

            var txt = await File.ReadAllTextAsync(categoryPath);
            return await _categoryParser.FromTextAsync(txt);
        }

        #endregion

        #region Blog Tag

        private async Task UpdateLocalTagsAsync()
        {
            var tags = await GetTagsFromFileAsync();
            _memoryCacheClient.Set(BlogConstant.TagMemCacheKey, tags, TimeSpan.FromDays(1));
        }

        private async Task<List<BlogTag>> GetTagsFromFileAsync()
        {
            var result = new List<BlogTag>();
            var tagPath = Path.Combine(_appConfig.AssetRepoLocalDir, BlogConstant.TagGitHubPath);
            if (!File.Exists(tagPath))
            {
                return result;
            }

            var txt = await File.ReadAllTextAsync(tagPath);
            return await _tagParser.FromTextAsync(txt);
        }

        #endregion

    }
}
