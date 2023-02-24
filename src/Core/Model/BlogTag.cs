using Swan.Core.Model.Object;
using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class BlogTag
    {
        public BlogTag() { }

        public BlogTag(BlogTagObject obj)
        {
            Object = obj;
        }

        [JsonPropertyName("o")]
        public BlogTagObject Object { get; init; }

        [JsonPropertyName("p")]
        public List<BlogPost> Posts { get; } = new();

        public string GetUrl()
        {
            return $"/blog/tag/{Object.Url}";
        }
    }
}
