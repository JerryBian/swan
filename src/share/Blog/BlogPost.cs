using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using HtmlAgilityPack;
using Laobian.Share.Util;
using Markdig;

namespace Laobian.Share.Blog
{
    public class BlogPost
    {
        [JsonIgnore] public bool IsPublished => Metadata.IsPublished && Metadata.PublishTime <= DateTime.Now;

        private void SetPostThumbnail(HtmlNode imageNode)
        {
            if (string.IsNullOrEmpty(Thumbnail) && !string.IsNullOrEmpty(imageNode.GetAttributeValue("src", null)))
            {
                Thumbnail = imageNode.OuterHtml;
            }
        }

        public void ExtractRuntimeData()
        {
            if (string.IsNullOrEmpty(MdContent))
            {
                MdContent = "Post content is empty.";
            }

            var html = Markdown.ToHtml(MdContent);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // all images nodes
            SetImageNodes(htmlDoc);
            HtmlContent = htmlDoc.DocumentNode.OuterHtml;

            // assign Excerpt
            SetExcerpt(htmlDoc);
        }

        private void SetExcerpt(HtmlDocument htmlDoc)
        {
            if (!string.IsNullOrEmpty(Metadata.Excerpt))
            {
                ExcerptHtml = Markdown.ToHtml(Metadata.Excerpt);
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
                        continue;
                    }

                    if (!Path.IsPathRooted(src))
                    {
                        var index = src.IndexOf(Constants.BlogPostFileBaseFolderName,
                            StringComparison.InvariantCultureIgnoreCase);
                        if (index >= 0)
                        {
                            var subPath = src.Substring(index + Constants.BlogPostFileBaseFolderName.Length + 1)
                                .Replace("\\", "/");
                            var fullSrc = $"/{Constants.BlogPostFileBaseUrlName}/{subPath}"; //TODO: Use Full Path for RSS
                            imageNode.SetAttributeValue("src", fullSrc);
                            SetPostThumbnail(imageNode);
                        }
                    }
                }
            }
        }

        public string GetFullPath(string baseAddress)
        {
            var path = $"{baseAddress}/{Metadata.PublishTime.Year:D4}/{Metadata.PublishTime.Month:D2}/{Metadata.Link}.html";
            return path;
        }

        #region Raw data

        [JsonPropertyName("link")] public string Link { get; set; }

        [JsonPropertyName("mdContent")] public string MdContent { get; set; }

        [JsonPropertyName("metadata")] public BlogMetadata Metadata { get; set; }

        [JsonPropertyName("tags")]
        public List<BlogTag> Tags { get; set; } = new();

        [JsonPropertyName("accesses")]
        public List<BlogAccess> Accesses { get; set; } = new();

        #endregion

        #region Runtime data

        [JsonPropertyName("htmlContent")] public string HtmlContent { get; set; }

        [JsonPropertyName("excerptHtml")] public string ExcerptHtml { get; set; }

        [JsonPropertyName("excerptPlainText")] public string ExcerptPlainText { get; set; }

        [JsonPropertyName("thumbnail")] public string Thumbnail { get; set; }

        #endregion
    }
}