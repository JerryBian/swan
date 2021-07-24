using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laobian.Blog
{
    public class GitHubCommit
    {
        [JsonPropertyName("message")] public string Message { get; set; }

        [JsonPropertyName("added")] public string[] Added { get; set; }

        [JsonPropertyName("modified")] public string[] Modified { get; set; }

        [JsonPropertyName("author")] public GitHubAuthor Author { get; set; }
    }
}
