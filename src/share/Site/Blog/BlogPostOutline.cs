using System.Text.Json.Serialization;

namespace Laobian.Share.Site.Blog
{
    public class BlogPostOutline
    {
        [JsonPropertyName("link")] public string Link { get; set; }

        [JsonPropertyName("title")] public string Title { get; set; }
    }
}