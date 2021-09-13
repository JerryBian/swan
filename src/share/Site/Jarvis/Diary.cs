using System;
using System.Text.Json.Serialization;
using Laobian.Share.Option;

namespace Laobian.Share.Site.Jarvis
{
    public class Diary
    {
        [JsonPropertyName("date")] public DateTime Date { get; set; }

        [JsonPropertyName("createTime")] public DateTime CreateTime { get; set; }

        [JsonPropertyName("lastUpdateTime")] public DateTime LastUpdateTime { get; set; }

        [JsonPropertyName("markdownContent")] public string MarkdownContent { get; set; }

        public string GetFullPath(LaobianSharedOption option)
        {
            return $"{option.JarvisRemoteEndpoint}/diary/{Date.Year:D4}/{GetDateString()}.html";
        }

        public string GetDateString()
        {
            return Date.ToString("yyyy-MM-dd");
        }
    }
}