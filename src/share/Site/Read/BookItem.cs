using System;
using System.Text.Json.Serialization;
using Laobian.Share.Util;

namespace Laobian.Share.Site.Read
{
    public class BookItem
    {
        [JsonPropertyName("id")] public string Id { get; set; } = StringUtil.GenerateRandom();

        [JsonPropertyName("bookName")] public string BookName { get; set; }

        [JsonPropertyName("bookName2")] public string BookName2 { get; set; }

        [JsonPropertyName("authorName")] public string AuthorName { get; set; }

        [JsonPropertyName("authorCountry")] public string AuthorCountry { get; set; }

        [JsonPropertyName("publisherName")] public string PublisherName { get; set; }

        [JsonPropertyName("publishTime")] public DateTime PublishTime { get; set; }

        [JsonPropertyName("startTime")] public DateTime StartTime { get; set; }

        [JsonPropertyName("endTime")] public DateTime EndTime { get; set; }

        [JsonPropertyName("isCompleted")] public bool IsCompleted { get; set; }

        [JsonPropertyName("blogPostLink")] public string BlogPostLink { get; set; }

        [JsonPropertyName("shortComment")] public string ShortComment { get; set; }

        [JsonPropertyName("grade")] public int Grade { get; set; }

        [JsonPropertyName("translator")] public string Translator { get; set; }

        [JsonPropertyName("lastUpdateTime")] public DateTime LastUpdateTime { get; set; }

        [JsonPropertyName("isPublished")] public bool IsPublished { get; set; }

        [JsonIgnore] public string ShortCommentHtml { get; set; }

        [JsonIgnore] public string BlogPostTitle { get; set; }
    }
}