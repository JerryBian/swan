using System.Text.Json.Serialization;

namespace Laobian.Share.Infrastructure.Git
{
    public class GitHubAuthor
    {
        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
