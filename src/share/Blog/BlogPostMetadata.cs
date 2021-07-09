using System;
using System.Collections.Generic;

namespace Laobian.Share.Blog
{
    public class BlogPostMetadata
    {
        public string Link { get; set; }

        public string Title { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime PublishTime { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public bool IsPublished { get; set; }

        public bool IsTopping { get; set; }

        public bool ContainsMath { get; set; }

        public bool AllowComment { get; set; }

        public string Excerpt { get; set; }

        public List<string> Tags { get; } = new();
    }
}