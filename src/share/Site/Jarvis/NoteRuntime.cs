using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using HtmlAgilityPack;
using Laobian.Share.Util;
using Markdig;

namespace Laobian.Share.Site.Jarvis;

public class NoteRuntime
{
    [JsonPropertyName("raw")] public Note Raw { get; set; }

    [JsonPropertyName("tags")] public List<NoteTag> Tags { get; set; } = new();

    [JsonPropertyName("htmlContent")] public string HtmlContent { get; set; }

    [JsonPropertyName("outline")] public List<NoteOutline> Outlines { get; set; } = new();

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
        var html = Markdown.ToHtml(Raw.MdContent);
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        // post outlines
        SetOutlines(htmlDoc);

        // all images nodes
        SetImageNodes(htmlDoc);
        HtmlContent = htmlDoc.DocumentNode.OuterHtml;

        // assign tags
        Tags.Clear();
        foreach (var tagLink in Raw.Tag)
        {
            var tag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(tagLink, x.Link));
            if (tag != null)
            {
                Tags.Add(tag);
            }
        }
    }

    private void SetImageNodes(HtmlDocument htmlDoc)
    {
        var imageNodes = htmlDoc.DocumentNode.Descendants("img").ToList();
        foreach (var imageNode in imageNodes)
        {
            if (imageNode.Attributes.Contains("src"))
            {
                imageNode.AddClass("img-thumbnail mx-auto d-block");
                imageNode.Attributes.Add("loading", "lazy");

                //var src = imageNode.Attributes["src"].Value;
                //if (string.IsNullOrEmpty(src))
                //{
                //    continue;
                //}

                //if (Uri.TryCreate(src, UriKind.Absolute, out var uriResult) &&
                //    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                //{
                //    // this is Network resources, keep it as it is
                //    SetPostThumbnail(imageNode);
                //}
            }
        }
    }
}