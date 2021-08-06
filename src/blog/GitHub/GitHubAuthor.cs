using System.Text.Json.Serialization;

namespace Laobian.Blog.GitHub
{
    public class GitHubAuthor
    {
        [JsonPropertyName("name")] public string User { get; set; }

        [JsonPropertyName("email")] public string Email { get; set; }
    }
}