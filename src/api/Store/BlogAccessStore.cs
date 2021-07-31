using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Laobian.Share.Blog;
using Laobian.Share.Helper;

namespace Laobian.Api.Store
{
    public class BlogAccessStore
    {
        private readonly ConcurrentDictionary<string, List<BlogAccess>> _access;

        public BlogAccessStore(IDictionary<string, string> val)
        {
            _access = new ConcurrentDictionary<string, List<BlogAccess>>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in val)
            {
                _access.TryAdd(item.Key, JsonHelper.Deserialize<List<BlogAccess>>(item.Value));
            }
        }

        public IDictionary<string, List<BlogAccess>> GetAll()
        {
            return _access;
        }

        public List<BlogAccess> GetByLink(string postLink)
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
                link => new List<BlogAccess> {new() {Count = count, Date = date}},
                (link, val) =>
                {
                    var access = val.FirstOrDefault(x => x.Date == date);
                    if (access == null)
                    {
                        access = new BlogAccess {Count = count, Date = date};
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