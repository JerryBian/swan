using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laobian.Share.Site.Jarvis
{
    public class Note
    {
        [JsonPropertyName("link")] public string Link { get; set; }

        [JsonPropertyName("mdContent")] public string MdContent { get; set; }

        [JsonPropertyName("title")] public string Title { get; set; }

        [JsonPropertyName("createTime")] public DateTime CreateTime { get; set; }

        [JsonPropertyName("lastUpdateTime")] public DateTime LastUpdateTime { get; set; }

        [JsonPropertyName("tag")] public List<string> Tag { get; set; } = new();

        public string GetFullPath()
        {
            var path =
                $"/note/{CreateTime.Year:D4}/{Link}.html";
            return path;
        }
    }
}
