using System;
using System.Collections.Generic;
using System.Threading;
using Laobian.Share.Extension;

namespace Laobian.Share.Blog.Model
{
    public class BlogPost
    {
        public BlogPost()
        {
            Raw = new BlogPostRaw();
            Categories = new List<BlogCategory>();
            Tags = new List<BlogTag>();
        }

        internal BlogPostRaw Raw { get; }

        #region Public Property

        private int _accessCount;
        public int AccessCount
        {
            get => _accessCount;
            set
            {
                _accessCount = value;
                Raw.AccessCount = _accessCount;
            }
        }

        public string Title => Raw.Title ?? throw new InvalidBlogAssetException(nameof(Title));

        public string AccessCountString => _accessCount.Human();

        public bool IsPublic => DateTime.Now > PublishTime && 
                                Raw.IsDraft != null &&
                                !Raw.IsDraft.Value;

        public DateTime CreateTime => Raw.CreateTime ?? throw new InvalidBlogAssetException(nameof(CreateTime));

        public DateTime PublishTime => Raw.PublishTime ?? CreateTime;

        public DateTime LastUpdateTime =>
            Raw.LastUpdateTime ?? throw new InvalidBlogAssetException(nameof(PublishTime));

        public string LastUpdateTimeString => LastUpdateTime.Human();

        public List<BlogCategory> Categories { get; }

        public List<BlogTag> Tags { get; }

        public string PublishTimeString => PublishTime.Human();

        public string ContentHtml { get; set; }

        public string ExcerptHtml { get; set; }

        public string ExcerptPlain { get; set; }

        public string FullUrl { get; set; }

        public string FullUrlWithBase { get; set; }

        public string GitPath { get; set; }

        public string LocalPath { get; set; }

        public string Link => Raw.Link;

        public BlogPost PrevPost { get; set; }

        public BlogPost NextPost { get; set; }

        public bool IsTopping => Raw.IsTopping ?? false;

        public bool ContainsMath => Raw.ContainsMath ?? false;

        #endregion

        #region Public Methods

        public void NewAccess()
        {
            Interlocked.Increment(ref _accessCount);
            Raw.AccessCount = _accessCount;
        }

        #endregion
    }
}
