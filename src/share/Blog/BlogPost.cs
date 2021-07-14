using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Laobian.Share.Blog
{
    public class BlogPost
    {
        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("mdContent")]
        public string MdContent { get; set; }

        [JsonPropertyName("htmlContent")]
        public string HtmlContent { get; set; }

        [JsonPropertyName("metadata")]
        public BlogPostMetadata Metadata { get; set; }

        [JsonPropertyName("tags")]
        public List<BlogTag> Tags { get; } = new();

        [JsonPropertyName("comments")]
        public List<BlogCommentItem> Comments { get; } = new();

        [JsonPropertyName("accesses")]
        public List<BlogPostAccess> Accesses { get; } = new();

        [JsonPropertyName("fullPath")]
        public string FullPath { get; set; }

        [JsonPropertyName("publishTimeString")]
        public string PublishTimeString { get; set; }

        [JsonIgnore]
        public bool IsPublished => Metadata.IsPublished && Metadata.PublishTime <= DateTime.Now;

        [JsonPropertyName("accessCountString")]
        public string AccessCountString { get; set; }

        [JsonPropertyName("accessCount")]
        public int AccessCount { get; set; }

        [JsonPropertyName("commentCountString")]
        public string CommentCountString { get; set; }

        [JsonPropertyName("commentCount")]
        public int CommentCount { get; set; }

        [JsonPropertyName("excerptHtml")]
        public string ExcerptHtml { get; set; }

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; }
    }
}