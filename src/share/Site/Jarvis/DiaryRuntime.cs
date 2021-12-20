using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Markdig;

namespace Laobian.Share.Site.Jarvis;

[DataContract]
public class DiaryRuntime
{
    public DiaryRuntime(){}

    public DiaryRuntime(Diary diary)
    {
        Raw = diary;
    }

    [DataMember(Order = 1)]
    [JsonPropertyName("raw")] public Diary Raw { get; set; }

    [DataMember(Order = 2)]
    [JsonPropertyName("htmlContent")] public string HtmlContent { get; set; }

    [DataMember(Order = 3)]
    public DiaryRuntime Prev { get; set; }

    [DataMember(Order = 4)]
    public DiaryRuntime Next { get; set; }

    public void ExtractRuntimeData()
    {
        if (!string.IsNullOrEmpty(Raw.MarkdownContent))
        {
            HtmlContent = Markdown.ToHtml(Raw.MarkdownContent);
        }
    }
}