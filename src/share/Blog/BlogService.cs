using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Blog.Asset;
using Laobian.Share.Blog.Model;
using Laobian.Share.Helper;

namespace Laobian.Share.Blog
{
    public class BlogService : IBlogService
    {
        private readonly IBlogAssetManager _blogAssetManager;
        private readonly SemaphoreSlim _semaphoreSlim;

        public BlogService(
            IBlogAssetManager blogAssetManager)
        {
            _blogAssetManager = blogAssetManager;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public List<BlogPost> GetPosts(
            bool onlyPublic = true, 
            bool publishTimeDesc = true,
            bool toppingPostsFirst = true)
        {
            IEnumerable<BlogPost> posts = _blogAssetManager.GetAllPosts();
            posts = onlyPublic ? posts.Where(p => p.IsPublic) : posts;
            posts = publishTimeDesc
                ? posts.OrderByDescending(p => p.PublishTime)
                : posts.OrderBy(p => p.Metadata.PublishTime);
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

        public List<BlogCategory> GetCategories(
            bool onlyPublic = true, 
            bool publishTimeDesc = true,
            bool toppingPostsFirst = true)
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

                if (category.Posts.Any())
                {
                    categories.Add(category);
                }
            }

            return categories;
        }

        public List<BlogTag> GetTags(
            bool onlyPublic = true, 
            bool publishTimeDesc = true, 
            bool toppingPostsFirst = true)
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

                if (tag.Posts.Any())
                {
                    tags.Add(tag);
                }
            }

            return tags;
        }

        public string GetAboutHtml()
        {
            return _blogAssetManager.GetAboutHtml();
        }

        public List<BlogArchive> GetArchives(
            bool onlyPublic = true, 
            bool publishTimeDesc = true,
            bool toppingPostsFirst = true)
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

                if (archive.Posts.Any())
                {
                    archives.Add(archive);
                }
            }

            return archives;
        }

        public async Task InitAsync(bool clone = true)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                var shouldContinue = true;
                if (clone)
                {
                    shouldContinue = await _blogAssetManager.PullFromGitHubAsync();
                }

                if (shouldContinue)
                {
                    shouldContinue = await _blogAssetManager.ParseAssetsToObjectsAsync();
                }

                if (shouldContinue)
                {
                    shouldContinue = await _blogAssetManager.SerializeAssetsToFilesAsync();
                }

                if (shouldContinue)
                {
                    await _blogAssetManager.PushToGitHubAsync(":tada: Server started");
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task GitHookAsync(List<string> postLinks)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                var shouldContinue = await _blogAssetManager.PullFromGitHubAsync();
                if (shouldContinue)
                {
                    shouldContinue = await _blogAssetManager.PullFromGitHubAsync();
                }

                var oldPosts = new List<BlogPost>(_blogAssetManager.GetAllPosts());
                if (shouldContinue)
                {
                    shouldContinue = await _blogAssetManager.ParseAssetsToObjectsAsync();
                }

                if (shouldContinue)
                {
                    shouldContinue = _blogAssetManager.MergePosts(oldPosts);
                }

                if (shouldContinue)
                {
                    shouldContinue = _blogAssetManager.UpdatePosts(postLinks);
                }

                if (shouldContinue)
                {
                    shouldContinue = await _blogAssetManager.SerializeAssetsToFilesAsync();
                }

                if (shouldContinue)
                {
                    await _blogAssetManager.PushToGitHubAsync(":sparkles: Hook happened");
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task UpdateGitHubAsync()
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                var shouldContinue = await _blogAssetManager.SerializeAssetsToFilesAsync();
                if (shouldContinue)
                {
                    await _blogAssetManager.PushToGitHubAsync(":wind_chime: Scheduled update or server stop");
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}