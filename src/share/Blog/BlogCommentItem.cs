using System;
using System.Text.Json.Serialization;

namespace Laobian.Share.Blog
{
    public class BlogCommentItem
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; }

        [JsonPropertyName("mdContent")]
        public string MdContent { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("isReviewed")]
        public bool IsReviewed { get; set; }

        [JsonPropertyName("isPublished")]
        public bool IsPublished { get; set; }

        [JsonPropertyName("lastUpdatedAt")]
        public DateTime LastUpdatedAt { get; set; }

        [JsonIgnore]
        public string IdString => Id.ToString("N");
    }
}