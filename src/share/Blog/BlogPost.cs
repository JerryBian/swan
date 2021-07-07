using System;
using System.Collections.Generic;

namespace Laobian.Share.Blog
{
    public class BlogPost
    {
        public string Link { get; set; }

        public string MdContent { get; set; }

        public string HtmlContent { get; set; }

        public BlogPostMetadata Metadata { get; set; }

        public List<BlogTag> Tags { get; set; }

        public List<BlogCommentItem> Comments { get; set; }

        public List<BlogPostAccess> Accesses { get; set; }

        public void LoadAdditionalInfo()
        {
            if (Metadata == null)
            {
                throw new InvalidOperationException("Metadata is not set yet.");
            }
        }
    }
}