using System;
using System.Text.Json.Serialization;

namespace Laobian.Share.Site.Blog
{
    public class BlogAccess
    {
        [JsonPropertyName("date")] public DateTime Date { get; set; }

        [JsonPropertyName("count")] public int Count { get; set; }
    }
}