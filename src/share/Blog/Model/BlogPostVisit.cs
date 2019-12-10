using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Laobian.Share.Blog.Model
{
    public class BlogPostVisit
    {
        private readonly ConcurrentDictionary<string, int> _postVisits;

        public BlogPostVisit()
        {
            _postVisits = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public bool ContainsKey(string key)
        {
            return _postVisits.ContainsKey(key);
        }

        public void Add(string key, int value)
        {
            _postVisits.TryAdd(key, value);
        }

        public int Get(string postPath)
        {
            return _postVisits.GetOrAdd(postPath, key => 0);
        }

        public void Update(string postPath)
        {
            _postVisits.AddOrUpdate(
                postPath, key => 0, 
                (key, current) => current + 1);
        }

        public void Update(string postPath, int value)
        {
            _postVisits.AddOrUpdate(
                postPath, key => value,
                (key, current) => value);
        }

        public IDictionary<string, int> Dump()
        {
            return _postVisits;
        }

        public BlogPostVisit Clone()
        {
            var result = new BlogPostVisit();
            foreach (var item in Dump())
            {
                result.Add(item.Key, item.Value);
            }

            return result;
        }
    }
}
