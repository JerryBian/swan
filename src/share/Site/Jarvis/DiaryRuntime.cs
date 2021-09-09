using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Markdig;

namespace Laobian.Share.Site.Jarvis
{
    public class DiaryRuntime
    {
        [JsonPropertyName("raw")]
        public Diary Raw { get; set; }

        [JsonPropertyName("htmlContent")]
        public string HtmlContent { get; set; }

        public void ExtractRuntimeData()
        {
            if (!string.IsNullOrEmpty(Raw.MarkdownContent))
            {
                HtmlContent = Markdown.ToHtml(Raw.MarkdownContent);
            }
        }
    }
}
