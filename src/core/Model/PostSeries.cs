using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class PostSeries : SwanObject
    {
        #region Raw

        [StoreUnique]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [StoreUnique]
        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        #endregion

        #region Extension

        [JsonIgnore]
        public List<SwanPost> BlogPosts { get; } = new();

        #endregion

        public override string GetFullLink() => $"/post/series#link-{Link}";

        public override string GetGitStorePath() => "obj/series.json";
    }
}
