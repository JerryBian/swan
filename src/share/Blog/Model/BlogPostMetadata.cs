using System;
using System.Collections.Generic;
using System.Threading;

namespace Laobian.Share.Blog.Model
{
    internal class BlogPostMetadata
    {
        private int _accessCount;

        public BlogPostMetadata()
        {
            _accessCount = 1;
        }

        public string Link { get; set; } = Guid.NewGuid().ToString("N");

        public string Title { get; set; } = Guid.NewGuid().ToString("N");

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime PublishTime { get; set; } = DateTime.Now;

        public DateTime LastUpdateTime { get; set; } = DateTime.Now;

        public bool IsDraft { get; set; } = true;

        public bool IsTopping { get; set; } = false;

        public bool ContainsMath { get; set; } = false;

        public List<string> Category { get; set; } = new List<string>();

        public List<string> Tag { get; set; } = new List<string>();

        public int AccessCount
        {
            get => _accessCount;
            set => _accessCount = value;
        }

        public void IncrementAccessCount()
        {
            Interlocked.Increment(ref _accessCount);
        }
    }
}