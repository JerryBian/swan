using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Blog.Asset;
using Laobian.Share.Blog.Model;
using Laobian.Share.Git;
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

            var allPosts = GetPosts(onlyPublic);
            var remainingPosts = allPosts.Except(categories.SelectMany(p => p.Posts)).ToList();
            if (remainingPosts.Any())
            {
                var cat = new BlogCategory {Link = "untitled", Name = "[未分类]"};
                cat.Posts.AddRange(remainingPosts);
                categories.Add(cat);
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

            var allPosts = GetPosts(onlyPublic);
            var remainingPosts = allPosts.Except(tags.SelectMany(p => p.Posts)).ToList();
            if (remainingPosts.Any())
            {
                var tag = new BlogTag {Link = "untitled", Name = "[无标签]"};
                tag.Posts.AddRange(remainingPosts);
                tags.Add(tag);
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
            foreach (var item in _blogAssetManager.GetAllPosts().ToLookup(p => p.PublishTime.Year)
                .OrderByDescending(x => x.Key))
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

        public async Task<string> InitAsync(bool clone = true)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                if (clone)
                {
                    await _blogAssetManager.PullFromGitHubAsync();
                }
                
                var messages = await _blogAssetManager.ParseAssetsToObjectsAsync();
                await _blogAssetManager.SerializeAssetsToFilesAsync();
                await _blogAssetManager.PushToGitHubAsync(GitCommitMessageFactory.ServerStarted());
                return messages;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<string> GitHookAsync(List<string> postLinks)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                await _blogAssetManager.PullFromGitHubAsync();

                var oldPosts = new List<BlogPost>(_blogAssetManager.GetAllPosts());
                var messages = await _blogAssetManager.ParseAssetsToObjectsAsync();
                _blogAssetManager.MergePosts(oldPosts);
                _blogAssetManager.UpdatePosts(postLinks);
                await _blogAssetManager.SerializeAssetsToFilesAsync();
                await _blogAssetManager.PushToGitHubAsync(GitCommitMessageFactory.GitHubHook());
                return messages;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task UpdateGitHubAsync(string commitMessage)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                await _blogAssetManager.SerializeAssetsToFilesAsync();
                await _blogAssetManager.PushToGitHubAsync(commitMessage);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}