using System;
using System.Collections.Generic;
using Laobian.Share.Extension;

namespace Laobian.Share.Blog.Model
{
    public class BlogPost
    {
        public BlogPost()
        {
            Categories = new List<BlogCategory>();
            Tags = new List<BlogTag>();
            Metadata = new BlogPostMetadata();
        }

        internal BlogPostMetadata Metadata { get; set; }

        #region Public Methods

        public DateTime? GetRawPublishTime()
        {
            return Metadata.PublishTime;
        }

        #endregion

        public void NewAccess()
        {
            Metadata.IncrementAccessCount();
        }

        #region Public Property

        public int AccessCount
        {
            get => Metadata.AccessCount;
            set => Metadata.AccessCount = value;
        }

        public string Title => Metadata.Title ?? throw new InvalidBlogAssetException(nameof(Title));

        public string AccessCountString => AccessCount.Human();

        public bool IsPublic => DateTime.Now > PublishTime &&
                                !Metadata.IsDraft;

        public DateTime CreateTime => Metadata.CreateTime;

        public DateTime PublishTime => Metadata.PublishTime;

        public DateTime LastUpdateTime => Metadata.LastUpdateTime;

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

        public string Link { get; set; }

        public bool IsTopping => Metadata.IsTopping;

        public bool ContainsMath => Metadata.ContainsMath;

        public string ContentMarkdown { get; set; }

        #endregion

        #region UI

        public string MetadataHtml { get; set; }

        public string CategoryAndTagHtml { get; set; }

        public string HeadDescription { get; set; }

        #endregion
    }
}