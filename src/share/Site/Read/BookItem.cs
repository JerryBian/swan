using System;
using System.Text.Json.Serialization;
using Laobian.Share.Converter;

namespace Laobian.Share.Site.Read;

public class BookItem
{
    [JsonPropertyOrder(1)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyOrder(2)]
    [JsonPropertyName("bookName")]
    public string BookName { get; set; }

    [JsonPropertyOrder(3)]
    [JsonPropertyName("bookName2")]
    public string BookName2 { get; set; }

    [JsonPropertyOrder(4)]
    [JsonPropertyName("authorName")]
    public string AuthorName { get; set; }

    [JsonPropertyOrder(5)]
    [JsonPropertyName("authorCountry")]
    public string AuthorCountry { get; set; }

    [JsonPropertyOrder(6)]
    [JsonPropertyName("publisherName")]
    public string PublisherName { get; set; }

    [JsonPropertyOrder(7)]
    [JsonPropertyName("publishTime")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateTime PublishTime { get; set; }

    [JsonPropertyOrder(8)]
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    [JsonPropertyOrder(9)]
    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }

    [JsonPropertyOrder(10)]
    [JsonPropertyName("isCompleted")]
    public bool IsCompleted { get; set; }

    [JsonPropertyOrder(11)]
    [JsonPropertyName("blogPostLink")]
    public string BlogPostLink { get; set; }

    [JsonPropertyOrder(12)]
    [JsonPropertyName("shortComment")]
    public string ShortComment { get; set; }

    [JsonPropertyOrder(13)]
    [JsonPropertyName("grade")]
    public int Grade { get; set; }

    [JsonPropertyOrder(14)]
    [JsonPropertyName("translator")]
    public string Translator { get; set; }

    [JsonPropertyOrder(15)]
    [JsonPropertyName("lastUpdateTime")]
    public DateTime LastUpdateTime { get; set; }

    [JsonPropertyOrder(16)]
    [JsonPropertyName("isPublished")]
    public bool IsPublished { get; set; }

    [JsonIgnore] public string ShortCommentHtml { get; set; }

    [JsonIgnore] public string BlogPostTitle { get; set; }
}