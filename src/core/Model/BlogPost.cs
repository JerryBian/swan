using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class BlogPost : SwanObject
    {
        public const string GitStorePath = "obj/blog/post.json";

        #region Raw

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("excerpt")]
        public string Excerpt { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("series")]
        public string Series { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();

        [JsonPropertyName("publishDate")]
        public DateTime PublishDate { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; } = false;

        #endregion

        #region Extension

        [JsonIgnore]
        public string HtmlContent { get; set; }

        [JsonIgnore]
        public BlogSeries BlogSeries { get; set; }

        [JsonIgnore]
        public List<BlogTag> BlogTags { get; } = new();

        [JsonIgnore]
        public PageStat PageStat { get; set; }

        #endregion

        public string GetFullLink()
        {
            return $"/blog/post/{Link}.html";
        }
    }
}
