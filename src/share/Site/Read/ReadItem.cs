using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Laobian.Share.Converter;

namespace Laobian.Share.Site.Read;

[DataContract]
public class ReadItem
{
    [DataMember(Order = 1)]
    [JsonPropertyOrder(1)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [DataMember(Order = 2)]
    [JsonPropertyOrder(2)]
    [JsonPropertyName("bookName")]
    public string BookName { get; set; }

    [DataMember(Order = 3)]
    [JsonPropertyOrder(3)]
    [JsonPropertyName("bookName2")]
    public string BookName2 { get; set; }

    [DataMember(Order = 4)]
    [JsonPropertyOrder(4)]
    [JsonPropertyName("authorName")]
    public string AuthorName { get; set; }

    [DataMember(Order = 5)]
    [JsonPropertyOrder(5)]
    [JsonPropertyName("authorCountry")]
    public string AuthorCountry { get; set; }

    [DataMember(Order = 6)]
    [JsonPropertyOrder(6)]
    [JsonPropertyName("publisherName")]
    public string PublisherName { get; set; }

    [DataMember(Order = 7)]
    [JsonPropertyOrder(7)]
    [JsonPropertyName("publishTime")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateTime PublishTime { get; set; }

    [DataMember(Order = 8)]
    [JsonPropertyOrder(8)]
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    [DataMember(Order = 9)]
    [JsonPropertyOrder(9)]
    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }

    [DataMember(Order = 10)]
    [JsonPropertyOrder(10)]
    [JsonPropertyName("isCompleted")]
    public bool IsCompleted { get; set; }

    [DataMember(Order = 11)]
    [JsonPropertyOrder(11)]
    [JsonPropertyName("blogPostLink")]
    public string BlogPostLink { get; set; }

    [DataMember(Order = 12)]
    [JsonPropertyOrder(12)]
    [JsonPropertyName("shortComment")]
    public string ShortComment { get; set; }

    [DataMember(Order = 13)]
    [JsonPropertyOrder(13)]
    [JsonPropertyName("grade")]
    public int Grade { get; set; }

    [DataMember(Order = 14)]
    [JsonPropertyOrder(14)]
    [JsonPropertyName("translator")]
    public string Translator { get; set; }

    [DataMember(Order = 15)]
    [JsonPropertyOrder(15)]
    [JsonPropertyName("lastUpdateTime")]
    public DateTime LastUpdateTime { get; set; }

    [DataMember(Order = 16)]
    [JsonPropertyOrder(16)]
    [JsonPropertyName("isPublished")]
    public bool IsPublished { get; set; }
}