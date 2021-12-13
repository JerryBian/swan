using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Laobian.Share.Converter;

namespace Laobian.Share.Site.Blog;

[DataContract]
public class BlogAccess
{
    [DataMember(Order = 1)]
    [JsonPropertyOrder(1)]
    [JsonPropertyName("date")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateTime Date { get; set; }

    [DataMember(Order = 2)]
    [JsonPropertyOrder(2)]
    [JsonPropertyName("count")]
    public int Count { get; set; }
}