using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laobian.Share.Site.Jarvis
{
    public class NoteTag
    {
        [JsonPropertyName("displayName")] public string DisplayName { get; set; }

        [JsonPropertyName("link")] public string Link { get; set; }

        [JsonPropertyName("description")] public string Description { get; set; }

        [JsonPropertyName("lastUpdatedAt")] public DateTime LastUpdatedAt { get; set; }
    }
}
