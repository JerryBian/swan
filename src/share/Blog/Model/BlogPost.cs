using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Humanizer;

namespace Laobian.Share.Blog.Model
{
    public class BlogPost
    {
        internal BlogPostRaw Raw { get; set; }

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

        public string AccessCountString => _accessCount.ToMetric();

        public bool IsPublic => DateTimeOffset.UtcNow > Raw.PublishTime?.UtcDateTime && 
                                Raw.IsDraft != null &&
                                !Raw.IsDraft.Value;

        public DateTimeOffset PublishTime => Raw.PublishTime ?? throw new InvalidBlogAssetException(nameof(PublishTime));

        public bool IsTopping => Raw.IsTopping ?? Raw.IsToppingDefault;

        public List<BlogCategory> Categories { get; set; }

        public string PublishTimeString => PublishTime.Humanize();

        public string ContentHtml { get; set; }

        public string ExcerptHtml { get; set; }

        public string ExcerptPlain { get; set; }

        public string FullUrl { get; set; }

        public string FullUrlWithBase { get; set; }

        public string GitPath { get; set; }

        public string LocalPath { get; set; }

        #endregion

        #region Public Methods

        public void NewAccess()
        {
            Interlocked.Increment(ref _accessCount);
        }

        public void Update()
        {
            var timeZoneOffset = Raw.TimeZoneOffset ?? Raw.TimeZoneOffsetDefault;
            Raw.LastUpdateTime = DateTimeOffset.UtcNow.AddHours(timeZoneOffset);
        }

        #endregion
    }
}
