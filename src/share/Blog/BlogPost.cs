using System;
using System.Collections.Generic;

namespace Laobian.Share.Blog
{
    public class BlogPost
    {
        public string Link { get; set; }

        public string MdContent { get; set; }

        public string HtmlContent { get; set; }

        public int TotalAccess { get; set; }

        public BlogPostMetadata Metadata { get; set; }

        public List<BlogTag> Tags { get; } = new();

        public List<BlogCommentItem> Comments { get; } = new();

        public List<BlogPostAccess> Accesses { get; } = new();

        public string FullPath { get; set; }

        public string PublishTimeString { get; set; }

        public bool IsPublished => Metadata.IsPublished && Metadata.PublishTime <= DateTime.Now;

        public string AccessCountString { get; set; }

        public string CommentCountString { get; set; }
    }
}