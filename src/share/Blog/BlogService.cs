using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog
{
    public class BlogService
    {
        private readonly BlogAssetManager _blogAssetManager;

        public BlogService()
        {
            _blogAssetManager = new BlogAssetManager();
        }

        public List<BlogPost> GetPosts(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true)
        {
            var posts = onlyPublic ? _blogAssetManager.AllPosts.Where(p => p.IsPublic) : _blogAssetManager.AllPosts;
            posts = publishTimeDesc
                ? posts.OrderByDescending(p => p.PublishTime)
                : posts.OrderBy(p => p.Raw.PublishTime);
            posts = toppingPostsFirst ? posts.OrderBy(p => p.IsTopping ? 0 : 1) : posts;
            return posts.ToList();
        }
    }
}
