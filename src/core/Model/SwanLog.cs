using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class SwanLog : SwanObject
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("level")]
        public string Level { get; set; }

        public override string GetGitStorePath() => "obj/_log.json";
    }
}
