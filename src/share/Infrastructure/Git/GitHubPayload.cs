using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Laobian.Share.Infrastructure.Git
{
    public class GitHubPayload
    {
        [JsonPropertyName("ref")]
        public string Ref { get; set; }

        [JsonPropertyName("commits")]
        public IEnumerable<GitHubCommit> Commits { get; set; }
    }
}
