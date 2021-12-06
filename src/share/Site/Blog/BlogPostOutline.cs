using System.Text.Json.Serialization;

namespace Laobian.Share.Site.Blog;

public class BlogPostOutline
{
    [JsonPropertyOrder(1)]
    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyOrder(2)]
    [JsonPropertyName("title")]
    public string Title { get; set; }
}