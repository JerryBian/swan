using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Swan.Core.Model2
{
    public class SwanPost : SwanObject<SwanPost>
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("tag_id")]
        public long TagId { get; set; }

        [JsonPropertyName("is_public")]
        public bool IsPublic { get; set; }

        [JsonPropertyName("visits")]
        public int Visits { get; set; }

        [JsonPropertyName("published_at")]
        public DateTime PublishedAt { get; set; }
    }
}
