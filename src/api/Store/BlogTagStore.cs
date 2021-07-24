using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Laobian.Share.Blog;
using Laobian.Share.Helper;

namespace Laobian.Api.Store
{
    public class BlogTagStore
    {
        private readonly ConcurrentDictionary<string, BlogTag> _tags;

        public BlogTagStore(string tags)
        {
            _tags = new ConcurrentDictionary<string, BlogTag>(StringComparer.InvariantCultureIgnoreCase);
            if (!string.IsNullOrEmpty(tags))
            {
                foreach (var blogTag in JsonHelper.Deserialize<List<BlogTag>>(tags))
                {
                    _tags.TryAdd(blogTag.Link, blogTag);
                }
            }
        }

        public BlogTag GetByLink(string link)
        {
            if (_tags.TryGetValue(link, out var val))
            {
                return val;
            }

            return null;
        }

        public List<BlogTag> GetAll()
        {
            return _tags.Values.ToList();
        }

        public void Add(BlogTag tag)
        {
            if (_tags.ContainsKey(tag.Link))
            {
                throw new InvalidOperationException($"Tag link already exists: {tag.Link}");
            }

            if (GetAll().FirstOrDefault(x => StringHelper.EqualIgnoreCase(x.DisplayName, tag.DisplayName)) != null)
            {
                throw new InvalidOperationException($"Tag name already exists: {tag.DisplayName}");
            }

            tag.LastUpdatedAt = DateTime.Now;
            _tags.TryAdd(tag.Link, tag);
        }

        public void Update(BlogTag tag)
        {
            var existingTag = GetByLink(tag.Link);
            if (existingTag == null)
            {
                throw new InvalidOperationException($"Tag link not exists: {tag.Link}");
            }

            if (GetAll().Except(new []{ existingTag }).FirstOrDefault(x => StringHelper.EqualIgnoreCase(x.DisplayName, tag.DisplayName)) != null)
            {
                throw new InvalidOperationException($"Tag name already exists: {tag.DisplayName}");
            }

            existingTag.Description = tag.Description;
            existingTag.DisplayName = tag.DisplayName;
            existingTag.LastUpdatedAt = DateTime.Now;
        }

        public void RemoveByLink(string link)
        {
            _tags.TryRemove(link, out _);
        }
    }
}
