using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Laobian.Share.Blog;
using Laobian.Share.Helper;

namespace Laobian.Api.Store
{
    public class BlogMetadataStore
    {
        private readonly ConcurrentDictionary<string, BlogPostMetadata> _allMetadata;

        public BlogMetadataStore(string metadata)
        {
            _allMetadata = new ConcurrentDictionary<string, BlogPostMetadata>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in JsonHelper.Deserialize<List<BlogPostMetadata>>(metadata))
            {
                _allMetadata.TryAdd(item.Link, item);
            }
        }

        public List<BlogPostMetadata> GetAll()
        {
            return _allMetadata.Values.ToList();
        }

        public BlogPostMetadata GetByLink(string postLink)
        {
            if (_allMetadata.TryGetValue(postLink, out var metadata))
            {
                return metadata;
            }

            return null;
        }

        public void Add(BlogPostMetadata metadata)
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

        public void Update(BlogPostMetadata metadata)
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
            val.AllowComment = metadata.AllowComment;
            val.ContainsMath = metadata.ContainsMath;
            val.Excerpt = metadata.Excerpt;
            val.IsTopping = metadata.IsTopping;
            val.Title = metadata.Title;
        }
    }
}
