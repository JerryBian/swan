using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Blog;
using Laobian.Share.Helper;

namespace Laobian.Api.Store
{
    public class BlogAccessStore
    {
        private readonly ConcurrentDictionary<string, List<BlogPostAccess>> _access;

        public BlogAccessStore(IDictionary<string, string> val)
        {
            _access = new ConcurrentDictionary<string, List<BlogPostAccess>>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in val)
            {
                _access.TryAdd(item.Key, JsonHelper.Deserialize<List<BlogPostAccess>>(item.Value));
            }
        }

        public IDictionary<string, List<BlogPostAccess>> GetAll()
        {
            return _access;
        }

        public List<BlogPostAccess> GetByLink(string postLink)
        {
            if (_access.TryGetValue(postLink, out var val))
            {
                return val;
            }

            return null;
        }

        public void Add(string postLink, DateTime date, int count)
        {
            _access.AddOrUpdate(postLink,
                link => new List<BlogPostAccess> {new BlogPostAccess {Count = count, Date = date}},
                (link, val) =>
                {
                    var access = val.FirstOrDefault(x => x.Date == date);
                    if (access == null)
                    {
                        access = new BlogPostAccess {Count = count, Date = date};
                        val.Add(access);
                    }
                    else
                    {
                        access.Count += count;
                    }
                    
                    return val;
                });
        }
    }
}
