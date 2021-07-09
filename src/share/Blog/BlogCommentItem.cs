using System;

namespace Laobian.Share.Blog
{
    public class BlogCommentItem
    {
        public Guid Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsAdmin { get; set; }

        public string MdContent { get; set; }

        public string IpAddress { get; set; }

        public bool IsReviewed { get; set; }

        public bool IsPublished { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public string IdString => Id.ToString("N");
    }
}