using System.Runtime.Serialization;
using Markdig;

namespace Laobian.Share.Site.Jarvis;

[DataContract]
public class DiaryRuntime
{
    public DiaryRuntime()
    {
    }

    public DiaryRuntime(Diary diary)
    {
        Raw = diary;
    }

    [DataMember(Order = 1)] public Diary Raw { get; set; }

    [DataMember(Order = 2)] public string HtmlContent { get; set; }

    [DataMember(Order = 3)] public DiaryRuntime Prev { get; set; }

    [DataMember(Order = 4)] public DiaryRuntime Next { get; set; }

    [DataMember(Order = 5)] public int WordsCount { get; set; }

    public void ExtractRuntimeData()
    {
        if (!string.IsNullOrEmpty(Raw.MarkdownContent))
        {
            HtmlContent = Markdown.ToHtml(Raw.MarkdownContent);
            WordsCount = Raw.MarkdownContent.Length;
        }
    }
}