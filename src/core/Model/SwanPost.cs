﻿using System.Text.Json.Serialization;

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
        public List<string> Tags { get; set; } = [];

        [JsonPropertyName("publishDate")]
        public DateTime PublishDate { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; } = false;

        #endregion

        #region Extension

        [JsonIgnore]
        public string TextExcerpt { get; set; }

        [JsonIgnore]
        public string HtmlContent { get; set; }

        [JsonIgnore]
        public PostSeries BlogSeries { get; set; }

        [JsonIgnore]
        public List<PostTag> BlogTags { get; } = [];

        [JsonIgnore]
        public SwanPage PageStat { get; set; } = new();

        [JsonIgnore]
        public SwanPost PreviousPost { get; set; }

        [JsonIgnore]
        public SwanPost NextPost { get; set; }

        [JsonIgnore]
        public List<SwanPost> RecommendPostsByTag { get; } = [];

        [JsonIgnore]
        public List<SwanPost> RecommendPostsBySeries { get; } = [];

        public string HtmlMetadata1 { get; set; }

        public string HtmlMetadata2 { get; set; }

        #endregion

        public override string GetGitStorePath() => "obj/post.json";

        public override bool IsPublicToEveryOne() => base.IsPublicToEveryOne() && !IsDeleted && PublishDate <= DateTime.Now;

        public override string GetFullLink() => $"/post/{Link}.html";
    }
}
