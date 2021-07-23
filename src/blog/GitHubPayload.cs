using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laobian.Blog
{
    public class GitHubPayload
    {
        [JsonPropertyName("ref")] public string Ref { get; set; }

        [JsonPropertyName("commits")] public IEnumerable<GitHubCommit> Commits { get; set; }
    }
}
