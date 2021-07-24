using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laobian.Blog
{
    public class GitHubAuthor
    {
        [JsonPropertyName("name")] public string User { get; set; }

        [JsonPropertyName("email")] public string Email { get; set; }
    }
}
