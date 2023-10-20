using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class PageStat : SwanObject
    {
        public const string GitFilePath = "obj/_page.json";

        [JsonPropertyName("hit")]
        public long Hit { get; set; }

        [JsonPropertyName("type")]
        public PageType PageType { get; set; }
    }
}
