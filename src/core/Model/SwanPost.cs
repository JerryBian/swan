using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class SwanPost : SwanObject
    {
        #region Raw

        [StoreUnique]
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [StoreUnique]
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
        public string HtmlExcerpt { get; set; }

        [JsonIgnore]
        public string HtmlContent { get; set; }

        [JsonIgnore]
        public PostSeries BlogSeries { get; set; }

        [JsonIgnore]
        public List<PostTag> BlogTags { get; } = new();

        [JsonIgnore]
        public PageStat PageStat { get; set; } = new();

        [JsonIgnore]
        public SwanPost PreviousPost { get; set; }

        [JsonIgnore]
        public SwanPost NextPost { get; set; }

        [JsonIgnore]
        public List<SwanPost> RecommendPostsByTag { get; } = new();

        [JsonIgnore]
        public List<SwanPost> RecommendPostsBySeries { get; } = new();

        [JsonIgnore]
        public string HtmlTag { get; set; }

        [JsonIgnore]
        public string HtmlSeries { get; set; }

        #endregion

        public override string GetGitStorePath() => "obj/post.json";

        public override bool IsPublicToEveryOne() => base.IsPublicToEveryOne() && !IsDeleted && PublishDate <= DateTime.Now;

        public override string GetFullLink() => $"/post/{Link}.html";
    }
}
