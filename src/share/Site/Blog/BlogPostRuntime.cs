using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HtmlAgilityPack;
using Laobian.Share.Util;
using Markdig;

namespace Laobian.Share.Site.Blog;

[DataContract]
public class BlogPostRuntime
{
    public BlogPostRuntime()
    {
    }

    public BlogPostRuntime(BlogPost raw)
    {
        Raw = raw;
    }

    [DataMember(Order = 1)] public BlogPost Raw { get; set; }

    [DataMember(Order = 2)] public List<BlogTag> Tags { get; set; } = new();

    [DataMember(Order = 3)] public List<BlogAccess> Accesses { get; set; } = new();

    [DataMember(Order = 4)] public string HtmlContent { get; set; }

    [DataMember(Order = 5)] public string ExcerptHtml { get; set; }

    [DataMember(Order = 6)] public string ExcerptPlainText { get; set; }

    [DataMember(Order = 7)] public string ThumbnailHtml { get; set; }

    [DataMember(Order = 8)] public string ThumbnailImageUrl { get; set; }

    [DataMember(Order = 9)] public List<BlogPostOutline> Outlines { get; set; } = new();

    [DataMember(Order = 10)] public int WordsCount { get; set; }

    private void SetPostThumbnail(HtmlNode imageNode)
    {
        if (string.IsNullOrEmpty(ThumbnailHtml) && !string.IsNullOrEmpty(imageNode.GetAttributeValue("src", null)))
        {
            ThumbnailHtml = imageNode.OuterHtml;
            ThumbnailImageUrl = imageNode.GetAttributeValue("src", null);
        }
    }

    public void ExtractRuntimeData(List<BlogAccess> access, List<BlogTag> tags)
    {
        if (string.IsNullOrEmpty(Raw.MdContent))
        {
            Raw.MdContent = "Post content is empty.";
        }

        WordsCount = Raw.MdContent.Length;

        var html = Markdown.ToHtml(Raw.MdContent);
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        // post outlines
        SetOutlines(htmlDoc);

        // all images nodes
        SetImageNodes(htmlDoc);
        HtmlContent = htmlDoc.DocumentNode.OuterHtml;

        // assign Excerpt
        SetExcerpt(htmlDoc);

        // assign access
        Accesses.Clear();
        if (access != null)
        {
            Accesses.AddRange(access);
        }

        // assign tags
        Tags.Clear();
        foreach (var tagId in Raw.Tag)
        {
            var tag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(tagId, x.Id));
            if (tag != null)
            {
                Tags.Add(tag);
            }
        }
    }

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
                var outline = new BlogPostOutline {Link = id, Title = item.InnerText};
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
                    var outline = new BlogPostOutline {Link = id, Title = item.InnerText};
                    item.Id = id;
                    Outlines.Add(outline);
                }
            }
        }
    }

    private void SetExcerpt(HtmlDocument htmlDoc)
    {
        if (!string.IsNullOrEmpty(Raw.Excerpt))
        {
            ExcerptHtml = Markdown.ToHtml(Raw.Excerpt);
        }
        else
        {
            var excerpt = string.Empty;
            var excerptText = string.Empty;
            var paraNodes =
                htmlDoc.DocumentNode
                    .Descendants()
                    .Where(_ =>
                        StringUtil.EqualsIgnoreCase(_.Name, "p") &&
                        _.Descendants().FirstOrDefault(c => StringUtil.EqualsIgnoreCase(c.Name, "img")) == null)
                    .Take(2)
                    .ToList();
            if (paraNodes.Count == 1)
            {
                excerpt += $"<p>{paraNodes[0].InnerText}</p>";
                excerptText += paraNodes[0].InnerText;
            }

            if (paraNodes.Count == 2)
            {
                excerpt += $"<p>{paraNodes[0].InnerText}</p><p>{paraNodes[1].InnerText}</p>";
                excerptText += $"{paraNodes[0].InnerText}{paraNodes[1].InnerText}";
            }

            ExcerptPlainText = excerptText;
            ExcerptHtml = excerpt;
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

                var src = imageNode.Attributes["src"].Value;
                if (string.IsNullOrEmpty(src))
                {
                    continue;
                }

                if (Uri.TryCreate(src, UriKind.Absolute, out var uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    // this is Network resources, keep it as it is
                    SetPostThumbnail(imageNode);
                }
            }
        }
    }

    public int GetAccessCount()
    {
        return Accesses.Sum(x => x.Count);
    }
}