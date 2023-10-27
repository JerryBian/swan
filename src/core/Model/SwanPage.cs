using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class SwanPage : SwanObject
    {
        public const string GitFilePath = "obj/_page.json";

        [JsonPropertyName("hit")]
        public long Hit { get; set; }

        [StoreUnique]
        [JsonPropertyName("url")]
        public string Path { get; set; }

        public override string GetGitStorePath() => "obj/_page.json";
    }
}
