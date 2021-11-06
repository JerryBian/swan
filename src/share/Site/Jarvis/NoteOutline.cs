using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laobian.Share.Site.Jarvis
{
    public class NoteOutline
    {
        [JsonPropertyName("link")] public string Link { get; set; }

        [JsonPropertyName("title")] public string Title { get; set; }
    }
}
