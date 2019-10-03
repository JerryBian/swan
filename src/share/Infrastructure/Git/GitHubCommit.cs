using System.Text.Json.Serialization;

namespace Laobian.Share.Infrastructure.Git
{
    public class GitHubCommit
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("added")]
        public string[] Added { get; set; }

        [JsonPropertyName("modified")]
        public string[] Modified { get; set; }
    }
}
