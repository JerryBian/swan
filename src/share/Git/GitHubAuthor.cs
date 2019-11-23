using System.Text.Json.Serialization;

namespace Laobian.Share.Git
{
    public class GitHubAuthor
    {
        [JsonPropertyName("name")] public string User { get; set; }

        [JsonPropertyName("email")] public string Email { get; set; }
    }
}