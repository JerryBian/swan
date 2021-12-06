using System;
using System.Text.Json.Serialization;
using Laobian.Share.Converter;

namespace Laobian.Share.Site.Blog;

public class BlogAccess
{
    [JsonPropertyOrder(1)]
    [JsonPropertyName("date")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateTime Date { get; set; }

    [JsonPropertyOrder(2)]
    [JsonPropertyName("count")]
    public int Count { get; set; }
}