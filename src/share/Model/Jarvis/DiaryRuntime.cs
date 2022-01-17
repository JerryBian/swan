using System.Linq;
using System.Runtime.Serialization;
using HtmlAgilityPack;
using Laobian.Share.Util;

namespace Laobian.Share.Model.Jarvis;

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

    [DataMember(Order = 5)] public string HtmlExcerpt { get; set; }

    public void ExtractRuntimeData()
    {
        if (!string.IsNullOrEmpty(Raw.MarkdownContent))
        {
            HtmlContent = MarkdownUtil.ToHtml(Raw.MarkdownContent);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(HtmlContent);
            var paraNodes =
                htmlDoc.DocumentNode
                    .Descendants()
                    .Where(_ =>
                        StringUtil.EqualsIgnoreCase(_.Name, "p") &&
                        _.Descendants().FirstOrDefault(c => StringUtil.EqualsIgnoreCase(c.Name, "img")) == null)
                    .Take(2)
                    .ToList();
            HtmlExcerpt = paraNodes.Count switch
            {
                1 => $"<p>{paraNodes[0].InnerText}</p>",
                2 => $"<p>{paraNodes[0].InnerText}</p><p>{paraNodes[1].InnerText}</p>",
                _ => HtmlExcerpt
            };
        }
    }
}