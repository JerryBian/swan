using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Blog.Alert;
using Laobian.Share.Blog.Asset;
using Laobian.Share.Blog.Model;
using Laobian.Share.Helper;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Blog
{
    public class BlogService : IBlogService
    {
        private readonly ILogger<BlogService> _logger;
        private readonly IBlogAlertService _blogAlertService;
        private readonly IBlogAssetManager _blogAssetManager;

        public BlogService(ILogger<BlogService> logger, IBlogAssetManager blogAssetManager, IBlogAlertService blogAlertService)
        {
            _logger = logger;
            _blogAssetManager = blogAssetManager;
            _blogAlertService = blogAlertService;
        }

        public List<BlogPost> GetPosts(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true)
        {
            IEnumerable<BlogPost> posts = _blogAssetManager.GetAllPosts();
            posts = onlyPublic ? posts.Where(p => p.IsPublic) : posts;
            posts = publishTimeDesc
                ? posts.OrderByDescending(p => p.PublishTime)
                : posts.OrderBy(p => p.Raw.PublishTime);
            posts = toppingPostsFirst ? posts.OrderBy(p => p.IsTopping ? 0 : 1) : posts;
            return posts.ToList();
        }

        public BlogPost GetPost(int year, int month, string link, bool onlyPublic = true)
        {
            IEnumerable<BlogPost> posts = _blogAssetManager.GetAllPosts();
            var post = onlyPublic
                ? posts.FirstOrDefault(p =>
                    p.IsPublic && p.PublishTime.Year == year && p.PublishTime.Month == month &&
                    CompareHelper.IgnoreCase(p.Link, link))
                : posts.FirstOrDefault(p =>
                    p.PublishTime.Year == year && p.PublishTime.Month == month &&
                    CompareHelper.IgnoreCase(p.Link, link));
            return post;
        }

        public List<BlogCategory> GetCategories(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true)
        {
            var categories = new List<BlogCategory>();
            foreach (var blogCategory in _blogAssetManager.GetAllCategories())
            {
                var category = new BlogCategory
                {
                    Link = blogCategory.Link,
                    Name = blogCategory.Name
                };

                IEnumerable<BlogPost> posts = new List<BlogPost>(blogCategory.Posts);
                if (onlyPublic)
                {
                    posts = posts.Where(p => p.IsPublic);
                }

                posts = publishTimeDesc
                    ? posts.OrderByDescending(p => p.PublishTime)
                    : posts.OrderBy(p => p.PublishTime);

                posts = toppingPostsFirst ? posts.OrderBy(p => p.IsTopping ? 0 : 1) : posts;
                category.Posts.AddRange(posts);
                categories.Add(category);
            }

            return categories;
        }

        public List<BlogTag> GetTags(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true)
        {
            var tags = new List<BlogTag>();
            foreach (var blogTag in _blogAssetManager.GetAllTags())
            {
                var tag = new BlogTag
                {
                    Link = blogTag.Link,
                    Name = blogTag.Name
                };

                IEnumerable<BlogPost> posts = new List<BlogPost>(blogTag.Posts);
                if (onlyPublic)
                {
                    posts = posts.Where(p => p.IsPublic);
                }

                posts = publishTimeDesc
                    ? posts.OrderByDescending(p => p.PublishTime)
                    : posts.OrderBy(p => p.PublishTime);

                posts = toppingPostsFirst ? posts.OrderBy(p => p.IsTopping ? 0 : 1) : posts;
                tag.Posts.AddRange(posts);
                tags.Add(tag);
            }

            return tags;
        }

        public string GetAboutHtml()
        {
            return _blogAssetManager.GetAboutHtml();
        }

        public List<BlogArchive> GetArchives(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true)
        {
            var archives = new List<BlogArchive>();
            foreach (var item in _blogAssetManager.GetAllPosts().ToLookup(p => p.PublishTime.Year))
            {
                var archive = new BlogArchive(item.Key);
                IEnumerable<BlogPost> posts = new List<BlogPost>(item);
                if (onlyPublic)
                {
                    posts = posts.Where(p => p.IsPublic);
                }

                posts = publishTimeDesc
                    ? posts.OrderByDescending(p => p.PublishTime)
                    : posts.OrderBy(p => p.PublishTime);

                posts = toppingPostsFirst ? posts.OrderBy(p => p.IsTopping ? 0 : 1) : posts;
                archive.Posts.AddRange(posts);
                archives.Add(archive);
            }

            return archives;
        }

        public async Task ReloadLocalAndMemoryAssetsAsync(
            bool clone = true, 
            bool updateTemplate = true, 
            List<string> addedPosts = null, 
            List<string> modifiedPosts = null)
        {
            try
            {
                if (clone)
                {
                    await _blogAssetManager.ReloadLocalFileStoreAsync();
                }

                if (updateTemplate)
                {
                    await _blogAssetManager.UpdateRemoteStoreTemplatePostAsync();
                }

                var reloadResult = await _blogAssetManager.UpdateMemoryStoreAsync();
                var reloadResultText = reloadResult.Success ? "SUCCESS!" : "FAIL!";
                var subject = $"Local and memory assets reload: {reloadResultText}";
                await _blogAlertService.AlertAssetReloadResultAsync(subject, reloadResult.Warning, reloadResult.Error,
                    addedPosts, modifiedPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reload local and memory assets throws error.");
                await _blogAlertService.AlertEventAsync("Reload local and memory assets throws error.", ex);
            }
        }

        public async Task UpdateRemoteStoreAsync()
        {
            await _blogAssetManager.UpdateLocalStoreAsync();
            await _blogAssetManager.UpdateRemoteStoreAsync();
        }

        public async Task MemoryToLocalStoreAsync()
        {
            await _blogAssetManager.UpdateLocalStoreAsync();
        }
    }
}
