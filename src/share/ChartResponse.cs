using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Laobian.Share
{
    public class ChartResponse
    {
        [JsonPropertyOrder(1)]
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyOrder(2)]
        [JsonPropertyName("labels")]
        public List<string> Labels { get; set; } = new();

        [JsonPropertyOrder(3)]
        [JsonPropertyName("data")]
        public List<int> Data { get; set; } = new();

        [JsonPropertyOrder(4)]
        [JsonPropertyName("type")]
        public string Type { get; set; } = "line";
    }
}
