using Swan.Core.Model.Object;
using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class BlogSeries
    {
        public BlogSeries(BlogSeriesObject obj)
        {
            Object = obj;
        }

        [JsonPropertyName("o")]
        public BlogSeriesObject Object { get; init; }

        [JsonPropertyName("p")]
        public List<BlogPost> Posts { get; } = new();

        public string GetUrl()
        {
            return $"/blog/series/{Object.Url}";
        }
    }
}
