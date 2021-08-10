using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Laobian.Share.Blog;
using Laobian.Share.Util;

namespace Laobian.Api.Store
{
    public class BlogMetadataStore
    {
        private readonly ConcurrentDictionary<string, BlogMetadata> _allMetadata;

        public BlogMetadataStore(string metadata)
        {
            _allMetadata =
                new ConcurrentDictionary<string, BlogMetadata>(StringComparer.InvariantCultureIgnoreCase);
            if (!string.IsNullOrEmpty(metadata))
            {
                foreach (var item in JsonUtil.Deserialize<List<BlogMetadata>>(metadata))
                {
                    _allMetadata.TryAdd(item.Link, item);
                }
            }
        }

        public List<BlogMetadata> GetAll()
        {
            return _allMetadata.Values.ToList();
        }

        public BlogMetadata GetByLink(string postLink)
        {
            if (_allMetadata.TryGetValue(postLink, out var metadata))
            {
                return metadata;
            }

            return null;
        }

        public void Add(BlogMetadata metadata)
        {
            if (_allMetadata.ContainsKey(metadata.Link))
            {
                throw new InvalidOperationException($"Metadata link already exists: {metadata.Link}");
            }

            _allMetadata.TryAdd(metadata.Link, metadata);
        }

        public void Remove(string postLink)
        {
            _allMetadata.TryRemove(postLink, out _);
        }

        public void Update(BlogMetadata metadata)
        {
            if (!_allMetadata.TryGetValue(metadata.Link, out var val))
            {
                throw new InvalidOperationException($"Metadata link not exists: {metadata.Link}");
            }

            val.IsPublished = metadata.IsPublished;
            val.LastUpdateTime = DateTime.Now;
            val.PublishTime = metadata.PublishTime;
            val.Tags.Clear();
            val.Tags.AddRange(metadata.Tags);
            val.ContainsMath = metadata.ContainsMath;
            val.Excerpt = metadata.Excerpt;
            val.IsTopping = metadata.IsTopping;
            val.Title = metadata.Title;
        }
    }
}