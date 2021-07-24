using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Laobian.Share.Blog;

namespace Laobian.Api.Store
{
    public class BlogPostStore
    {
        private readonly ConcurrentDictionary<string, BlogPost> _posts;

        public BlogPostStore(IDictionary<string, string> val)
        {
            _posts = new ConcurrentDictionary<string, BlogPost>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in val)
            {
                var post = new BlogPost {Link = item.Key, MdContent = item.Value};
                _posts.TryAdd(item.Key, post);
            }
        }

        public BlogPost GetByLink(string link)
        {
            if (_posts.TryGetValue(link, out var val))
            {
                return val;
            }

            return null;
        }

        public List<BlogPost> GetAll()
        {
            return _posts.Values.ToList();
        }
    }
}