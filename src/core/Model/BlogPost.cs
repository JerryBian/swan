using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class BlogPost : SwanObject
    {
        public const string GitStorePath = "obj/post.json";

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
        public string HtmlExcerpt { get; set; }

        [JsonIgnore]
        public string HtmlContent { get; set; }

        [JsonIgnore]
        public BlogSeries BlogSeries { get; set; }

        [JsonIgnore]
        public List<BlogTag> BlogTags { get; } = new();

        [JsonIgnore]
        public PageStat PageStat { get; set; } = new();

        [JsonIgnore]
        public BlogPost PreviousPost { get; set; }

        [JsonIgnore]
        public BlogPost NextPost { get; set; }

        [JsonIgnore]
        public List<BlogPost> RecommendPostsByTag { get; } = new();

        [JsonIgnore]
        public List<BlogPost> RecommendPostsBySeries { get; } = new();

        [JsonIgnore]
        public bool IsPublicToEveryOne => !IsDeleted && IsPublic && DateTime.Now >= PublishDate;

        #endregion

        public string GetFullLink()
        {
            return $"/post/{PublishDate.Year}/{Link}";
        }

        public string GetTagsHtml()
        {
            if(BlogTags == null || !BlogTags.Any())
            {
                return string.Empty;
            }

            return $"<i class=\"bi bi-tag\"></i> {string.Join(" ", BlogTags.Select(x => $"<a href=\"{x.GetFullLink()}\" class=\"text-reset text-decoration-none\">{x.Name}</a>"))}";
        }

        public string GetSeriesHtml()
        {
            if(BlogSeries == null)
            {
                return string.Empty;
            }

            return $"<i class=\"bi bi-bookmark\"></i> <a href=\"{BlogSeries.GetFullLink()}\" class=\"text-reset text-decoration-none\">{BlogSeries.Name}</a>";
        }

        public string GetTagsAndSeriesHtml()
        {
            var tagsHtml = GetTagsHtml();
            var seriesHtml = GetSeriesHtml();

            if(string.IsNullOrEmpty(tagsHtml) && string.IsNullOrEmpty(seriesHtml))
            {
                return string.Empty;
            }

            return $"<span class=\"my-3 mx-2 small\">{tagsHtml} {seriesHtml}</span>";
        }
    }
}
