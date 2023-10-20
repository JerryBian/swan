using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class SwanLog : SwanObject
    {
        public const string GitStorePath = "obj/_log.json";

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("level")]
        public string Level { get; set; }
    }
}
