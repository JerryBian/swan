using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HtmlAgilityPack;
using Laobian.Share.Util;

namespace Laobian.Share.Model.Jarvis;

[DataContract]
public class NoteRuntime
{
    public NoteRuntime()
    {
    }

    public NoteRuntime(Note note)
    {
        Raw = note;
    }

    [DataMember(Order = 1)] public Note Raw { get; set; }

    [DataMember(Order = 2)] public List<NoteTag> Tags { get; set; } = new();

    [DataMember(Order = 3)] public string HtmlContent { get; set; }

    [DataMember(Order = 4)] public List<NoteOutline> Outlines { get; set; } = new();

    [DataMember(Order = 5)] public string HtmlExcerpt { get; set; }

    private void SetOutlines(HtmlDocument htmlDoc)
    {
        var i = 0;
        var h3 = htmlDoc.DocumentNode.ChildNodes.Where(x => StringUtil.EqualsIgnoreCase(x.Name, "h3")).ToList();
        if (h3.Any())
        {
            foreach (var item in h3)
            {
                i++;
                var id = $"outline-1-{i}";
                var outline = new NoteOutline {Link = id, Title = item.InnerText};
                item.Id = id;
                Outlines.Add(outline);
            }
        }
        else
        {
            var h4 = htmlDoc.DocumentNode.ChildNodes.Where(x => StringUtil.EqualsIgnoreCase(x.Name, "h4")).ToList();
            if (h4.Any())
            {
                foreach (var item in h4)
                {
                    i++;
                    var id = $"outline-2-{i}";
                    var outline = new NoteOutline {Link = id, Title = item.InnerText};
                    item.Id = id;
                    Outlines.Add(outline);
                }
            }
        }
    }

    public void ExtractRuntimeData(List<NoteTag> tags)
    {
        var html = MarkdownUtil.ToHtml(Raw.MdContent);
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        // post outlines
        SetOutlines(htmlDoc);

        // all images nodes
        SetImageNodes(htmlDoc);
        SetTableNodes(htmlDoc);
        HtmlContent = htmlDoc.DocumentNode.OuterHtml;

        // assign tags
        Tags.Clear();
        foreach (var tagId in Raw.Tags)
        {
            var tag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(tagId, x.Id));
            if (tag != null)
            {
                Tags.Add(tag);
            }
        }

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

    private static void SetTableNodes(HtmlDocument htmlDoc)
    {
        var tableNodes = htmlDoc.DocumentNode.Descendants("table").ToList();
        foreach (var tableNode in tableNodes)
        {
            tableNode.AddClass("table table-striped table-bordered table-responsive");
        }
    }

    private static void SetImageNodes(HtmlDocument htmlDoc)
    {
        var imageNodes = htmlDoc.DocumentNode.Descendants("img").ToList();
        foreach (var imageNode in imageNodes)
        {
            if (imageNode.Attributes.Contains("src"))
            {
                imageNode.AddClass("img-thumbnail mx-auto d-block");
                imageNode.Attributes.Add("loading", "lazy");
            }
        }
    }

    public string GetTagHtml()
    {
        if (Tags == null)
        {
            return null;
        }

        return string.Join(" ",
            Tags.Select(x =>
                $"<a href=\"/note/tag/{x.Link}\"><span class=\"badge bg-primary\">{x.DisplayName}</span></a>"));
    }
}