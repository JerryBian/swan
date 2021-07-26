using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Laobian.Share.Blog
{
    public class BlogPostMetadata
    {
        [JsonPropertyName("link")] public string Link { get; set; }

        [JsonPropertyName("title")] public string Title { get; set; }

        [JsonPropertyName("createTime")] public DateTime CreateTime { get; set; }

        [JsonPropertyName("publishTime")] public DateTime PublishTime { get; set; }

        [JsonPropertyName("lastUpdateTime")] public DateTime LastUpdateTime { get; set; }

        [JsonPropertyName("isPublished")] public bool IsPublished { get; set; }

        [JsonPropertyName("isTopping")] public bool IsTopping { get; set; }

        [JsonPropertyName("containsMath")] public bool ContainsMath { get; set; }

        [JsonPropertyName("allowComment")] public bool AllowComment { get; set; }

        [JsonPropertyName("excerpt")] public string Excerpt { get; set; }

        [JsonInclude]
        [JsonPropertyName("tags")]
        public List<string> Tags { get; init; } = new();

        public void SetDefault()
        {
            Title = Guid.NewGuid().ToString("N");
            CreateTime = PublishTime = LastUpdateTime = DateTime.Now;
            IsPublished = IsTopping = ContainsMath = false;
            AllowComment = true;
        }
    }
}