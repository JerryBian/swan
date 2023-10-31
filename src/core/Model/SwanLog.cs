using Microsoft.Extensions.Logging;
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

        public string GetLogClassName()
        {
            var name = string.Empty;
            if (Level == LogLevel.Warning.ToString())
            {
                name = "text-bg-warning";
            }
            else if (Level == LogLevel.Error.ToString())
            {
                name = "text-bg-danger";
            }
            else if (Level == LogLevel.Debug.ToString())
            {
                name = "text-bg-light";
            }

            return name;
        }
    }
}
