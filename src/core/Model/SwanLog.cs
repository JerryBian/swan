using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class SwanLog : ISwanObject
    {
        #region Object

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("last_modified_at")]
        public DateTime LastModifiedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("ip_address")]
        public string IpAddress { get; set; }

        [JsonPropertyName("user_agent")]
        public string UserAgent { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("exception")]
        public string Exception { get; set; }

        [JsonPropertyName("level")]
        public string Level { get; set; }

        #endregion

        public static string GitPath => "data/_log.json";
    }
}
