using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Blog.Asset;
using Laobian.Share.Blog.Model;
using Laobian.Share.Helper;

namespace Laobian.Share.Blog
{
    public class BlogService : IBlogService
    {
        private readonly IBlogAssetManager _blogAssetManager;

        public BlogService(IBlogAssetManager blogAssetManager)
        {
            _blogAssetManager = blogAssetManager;
        }

        public async Task<List<BlogPost>> GetPostsAsync(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true)
        {
            IEnumerable<BlogPost> posts = await _blogAssetManager.GetAllPostsAsync();
            posts = onlyPublic ? posts.Where(p => p.IsPublic) : posts;
            posts = publishTimeDesc
                ? posts.OrderByDescending(p => p.PublishTime)
                : posts.OrderBy(p => p.Raw.PublishTime);
            posts = toppingPostsFirst ? posts.OrderBy(p => p.IsTopping ? 0 : 1) : posts;
            return posts.ToList();
        }

        public async Task<BlogPost> GetPostAsync(int year, int month, string link, bool onlyPublic = true)
        {
            IEnumerable<BlogPost> posts = await _blogAssetManager.GetAllPostsAsync();
            var post = onlyPublic
                ? posts.FirstOrDefault(p =>
                    p.IsPublic && p.PublishTime.Year == year && p.PublishTime.Month == month &&
                    CompareHelper.IgnoreCase(p.Link, link))
                : posts.FirstOrDefault(p =>
                    p.PublishTime.Year == year && p.PublishTime.Month == month &&
                    CompareHelper.IgnoreCase(p.Link, link));
            return post;
        }

        public async Task<List<BlogCategory>> GetCategoriesAsync(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true)
        {
            var categories = new List<BlogCategory>();
            foreach (var blogCategory in await _blogAssetManager.GetAllCategoriesAsync())
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

        public async Task<List<BlogTag>> GetTagsAsync(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true)
        {
            var tags = new List<BlogTag>();
            foreach (var blogTag in await _blogAssetManager.GetAllTagsAsync())
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

        public async Task<string> GetAboutHtmlAsync()
        {
            return await _blogAssetManager.GetAboutHtmlAsync();
        }

        public async Task<List<BlogArchive>> GetArchivesAsync(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true)
        {
            var archives = new List<BlogArchive>();
            foreach (var item in (await _blogAssetManager.GetAllPostsAsync()).ToLookup(p => p.PublishTime.Year))
            {
                var archive = new BlogArchive($"{item.Key} 年");
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

        public async Task ReloadLocalAssetsAsync(bool clone = true, bool updateTemplate = true)
        {
            if (clone)
            {
                await _blogAssetManager.ReloadLocalFileStoreAsync();
            }

            if (updateTemplate)
            {
                await _blogAssetManager.UpdateRemoteStoreTemplatePostAsync();
            }

            await _blogAssetManager.UpdateMemoryStoreAsync();
        }

        public async Task UpdateRemoteStoreAsync()
        {
            await _blogAssetManager.UpdateLocalStoreAsync();
            await _blogAssetManager.UpdateRemoteStoreAsync();
        }
    }
}
