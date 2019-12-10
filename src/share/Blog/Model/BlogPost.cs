using System;
using System.Collections.Generic;
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

        #region Public Methods

        public DateTime? GetRawPublishTime()
        {
            return Raw.PublishTime;
        }

        #endregion

        #region Public Property

        public int AccessCount { get; set; }

        public string Title => Raw.Title ?? throw new InvalidBlogAssetException(nameof(Title));

        public string AccessCountString => AccessCount.Human();

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

        public bool IsTopping => Raw.IsTopping ?? false;

        public bool ContainsMath => Raw.ContainsMath ?? false;

        #endregion
    }
}