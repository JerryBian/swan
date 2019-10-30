using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine.Model;
using Laobian.Share.BlogEngine.Parser;
using Laobian.Share.Config;
using Laobian.Share.Extension;
using Laobian.Share.Helper;
using Laobian.Share.Infrastructure.Cache;
using Laobian.Share.Infrastructure.Email;
using Laobian.Share.Infrastructure.Git;
using Laobian.Share.Log;
using Markdig;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Share.BlogEngine
{
    public class BlogService : IBlogService
    {
        private readonly GitConfig _gitConfig;
        private readonly AppConfig _appConfig;
        private readonly BlogTagParser _tagParser;
        private readonly BlogPostParser _postParser;
        private readonly IGitClient _gitClient;
        private readonly BlogCategoryParser _categoryParser;
        private readonly IMemoryCacheClient _memoryCacheClient;
        private readonly ILogService _logService;
        private readonly IEmailClient _emailClient;

        public BlogService(
            ILogService logService,
            IOptions<AppConfig> appConfig,
            IGitClient gitClient,
            IMemoryCacheClient memoryCacheClient,
            IEmailClient emailClient)
        {
            _logService = logService;
            _appConfig = appConfig.Value;
            _gitClient = gitClient;
            _emailClient = emailClient;
            _memoryCacheClient = memoryCacheClient;

            _tagParser = new BlogTagParser(_appConfig);
            _postParser = new BlogPostParser(_appConfig);
            _categoryParser = new BlogCategoryParser(_appConfig);
            _gitConfig = new GitConfig
            {
                GitHubRepositoryName = _appConfig.Blog.AssetGitHubRepoName,
                GitHubRepositoryBranch = _appConfig.Blog.AssetGitHubRepoBranch,
                GitHubRepositoryOwner = _appConfig.Blog.AssetGitHubRepoOwner,
                GitHubAccessToken = _appConfig.Blog.AssetGitHubRepoApiToken,
                GitCloneToDir = _appConfig.Blog.AssetRepoLocalDir,
                GitCommitEmail = _appConfig.Blog.AssetGitCommitEmail,
                GitCommitUser = _appConfig.Blog.AssetGitCommitUser
            };
        }

        #region Public Interface

        public string GetAboutHtml(RequestLang lang = RequestLang.Chinese)
        {
            var cacheKey = CacheKeyBuilder.Build(SiteComponent.Blog, "about", lang);
            if (!_memoryCacheClient.TryGet<string>(cacheKey, out var html))
            {
                html = string.Empty;
            }

            return html;
        }

        public BlogPost GetPost(int year, int month, string link)
        {
            var posts = GetPosts();
            var post = posts.FirstOrDefault(p =>
                p.CreationTimeUtc.Year == year &&
                p.CreationTimeUtc.Month == month &&
                StringEqualsHelper.IgnoreCase(p.Link, link));
            post?.AddVisit();

            return post;
        }

        public List<BlogPost> GetPosts(bool filterPublish, ref int page, out int totalPages)
        {
            var posts = filterPublish ? GetPublishedPosts() : GetPosts();
            posts = posts.OrderByDescending(p => p.CreationTimeUtc).ToList();
            totalPages = (int)Math.Ceiling(posts.Count / (double)_appConfig.Blog.PostsPerPage);
            if (page < 0)
            {
                _logService.LogWarning($"Request paged published posts with {page}");
            }

            page = Math.Max(page, 1);

            if (page > totalPages)
            {
                _logService.LogWarning($"Request paged published posts with {page}");
                page = totalPages;
            }

            return posts.ToPaged(_appConfig.Blog.PostsPerPage, page);
        }

        public List<BlogPost> GetPublishedPosts()
        {
            var posts = GetPosts();
            return posts.Where(_ => _.IsReallyPublic).OrderByDescending(_ => _.CreationTimeUtc).ToList();
        }

        public List<BlogPost> GetPosts()
        {
            if (!_memoryCacheClient.TryGet<List<BlogPost>>(CacheKeyBuilder.Build(SiteComponent.Blog, "post", "all"), out var posts))
            {
                posts = new List<BlogPost>();
            }

            return posts;
        }

        public List<BlogCategory> GetCategories()
        {
            if (!_memoryCacheClient.TryGet<List<BlogCategory>>(CacheKeyBuilder.Build(SiteComponent.Blog, "category", "all"), out var categories))
            {
                categories = new List<BlogCategory>();
            }

            return categories;
        }

        public List<BlogTag> GetTags()
        {
            if (!_memoryCacheClient.TryGet<List<BlogTag>>(CacheKeyBuilder.Build(SiteComponent.Blog, "tag", "all"), out var tags))
            {
                tags = new List<BlogTag>();
            }

            return tags;
        }

        public async Task UpdateCloudAssetsAsync()
        {
            await UpdateCloudPostsAsync();
        }

        public async Task UpdateMemoryAssetsAsync(bool cloneFirst = true)
        {
            if (cloneFirst)
            {
                await _gitClient.CloneAsync(_gitConfig);
                await UpdatePostTemplateAsync();
            }

            var tasks = new List<Task>
            {
                UpdateMemoryCategoriesAsync(),
                UpdateMemoryPostsAsync(),
                UpdateMemoryTagsAsync(),
                UpdateMemoryAboutAsync()
            };

            await Task.WhenAll(tasks);
        }

        #endregion

        #region Blog Post

        private async Task UpdatePostTemplateAsync()
        {
            var templatePost = new BlogPost
            {
                CreationTimeUtc = new DateTime(2008, 9, 1),
                LastUpdateTimeUtc = new DateTime(2019, 09, 01),
                IsPublic = false,
                Link = "Your-Post-Link-Here",
                Title = "Your Post Title Here",
                Visits = 10,
                CategoryNames = new List<string> { "分类名称1", "分类名称2" },
                TagNames = new List<string> { "标签名称1", "标签名称2" },
                MarkdownContent = "Your Post Content Here in Markdown."
            };

            var templateMarkdown = await new BlogPostParser(_appConfig).ToTextAsync(templatePost, false);
            var localTemplatePath = Path.Combine(_appConfig.Blog.AssetRepoLocalDir, _appConfig.Blog.TemplatePostGitPath);
            await File.WriteAllTextAsync(localTemplatePath, templateMarkdown, Encoding.UTF8);
            await _gitClient.CommitAsync(_appConfig.Blog.AssetRepoLocalDir, GitHubMessageProvider.GetPostCommitMessage($"{_appConfig.Blog.AssetGitCommitUser} - update template post from server"));
        }

        private async Task UpdateCloudPostsAsync()
        {
            var posts = GetPosts();
            foreach (var post in posts)
            {
                var postContent = await _postParser.ToTextAsync(post, true);
                await File.WriteAllTextAsync(post.LocalFullPath, postContent, Encoding.UTF8);
            }

            await _gitClient.CommitAsync(_appConfig.Blog.AssetRepoLocalDir,
                GitHubMessageProvider.GetPostCommitMessage(
                    $"{_appConfig.Blog.AssetGitCommitUser} - update posts from server"));
        }

        private async Task UpdateMemoryPostsAsync()
        {
            var memPosts = GetPosts();
            var filePosts = await GetPostsFromFileAsync();
            var posts = new List<BlogPost>();
            foreach (var filePost in filePosts)
            {
                if (StringEqualsHelper.IgnoreCase(filePost.GitHubPath, _appConfig.Blog.TemplatePostGitPath))
                {
                    continue;
                }

                var memPost = memPosts.FirstOrDefault(p =>
                    StringEqualsHelper.IgnoreCase(p.Link, filePost.Link));
                if (memPost != null)
                {
                    filePost.Visits = Math.Max(filePost.Visits, memPost.Visits);
                }

                posts.Add(filePost);
            }

            _memoryCacheClient.Set(CacheKeyBuilder.Build(SiteComponent.Blog, "post", "all"), posts);
        }

        private async Task<List<BlogPost>> GetPostsFromFileAsync()
        {
            var result = new List<BlogPost>();
            var postDirPath = Path.Combine(_appConfig.Blog.AssetRepoLocalDir, _appConfig.Blog.PostGitPath);
            if (!Directory.Exists(postDirPath))
            {
                return result;
            }

            foreach (var postItem in Directory.EnumerateFiles(
                postDirPath,
                $"*{_appConfig.Common.MarkdownExtension}"))
            {
                try
                {
                    var post = await _postParser.FromTextAsync(await File.ReadAllTextAsync(postItem), Path.GetFileNameWithoutExtension(postItem));
                    result.Add(post);
                }
                catch (Exception ex)
                {
                    _logService.LogError( $"Parse post failed. Post full path = {postItem}.", ex);
                }
            }

            return result;
        }

        #endregion

        #region Blog Category

        private async Task UpdateMemoryCategoriesAsync()
        {
            var categories = await GetCategoriesFromFileAsync();
            _memoryCacheClient.Set(CacheKeyBuilder.Build(SiteComponent.Blog, "category", "all"), categories);
        }

        private async Task<List<BlogCategory>> GetCategoriesFromFileAsync()
        {
            var result = new List<BlogCategory>();
            var categoryPath = Path.Combine(_appConfig.Blog.AssetRepoLocalDir, _appConfig.Blog.CategoryGitPath);
            if (!File.Exists(categoryPath))
            {
                return result;
            }

            var txt = await File.ReadAllTextAsync(categoryPath);
            return await _categoryParser.FromTextAsync(txt);
        }

        #endregion

        #region Blog Tag

        private async Task UpdateMemoryTagsAsync()
        {
            var tags = await GetTagsFromFileAsync();
            _memoryCacheClient.Set(CacheKeyBuilder.Build(SiteComponent.Blog, "tag", "all"), tags);
        }

        private async Task<List<BlogTag>> GetTagsFromFileAsync()
        {
            var result = new List<BlogTag>();
            var tagPath = Path.Combine(_appConfig.Blog.AssetRepoLocalDir, _appConfig.Blog.TagGitPath);
            if (!File.Exists(tagPath))
            {
                return result;
            }

            var txt = await File.ReadAllTextAsync(tagPath);
            return await _tagParser.FromTextAsync(txt);
        }

        #endregion

        #region Blog About

        private async Task UpdateMemoryAboutAsync()
        {
            var zhAbout = await GetAboutFromFileAsync(RequestLang.Chinese);
            _memoryCacheClient.Set(CacheKeyBuilder.Build(SiteComponent.Blog, "about", RequestLang.Chinese), zhAbout);

            var enAbout = await GetAboutFromFileAsync(RequestLang.English);
            _memoryCacheClient.Set(CacheKeyBuilder.Build(SiteComponent.Blog, "about", RequestLang.English), enAbout);
        }

        private async Task<string> GetAboutFromFileAsync(RequestLang lang)
        {
            var aboutPath = Path.Combine(_appConfig.Blog.AssetRepoLocalDir, _appConfig.Blog.AboutGitPath);
            if (!File.Exists(aboutPath))
            {
                return string.Empty;
            }

            var md = await File.ReadAllTextAsync(aboutPath);
            return Markdown.ToHtml(md);
        }

        #endregion

    }
}
