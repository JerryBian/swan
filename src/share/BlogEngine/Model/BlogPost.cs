using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using HtmlAgilityPack;

namespace Laobian.Share.BlogEngine.Model
{
    public class BlogPost
    {
        public BlogPost()
        {
            CategoryNames = new List<string>();
            TagNames = new List<string>();
        }

        #region Post Metadata

        public string Link { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.DateTime, "CreateAt")]
        public DateTime CreationTimeUtc { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.DateTime, "UpdateAt", IsAssignable = false)]
        public DateTime LastUpdateTimeUtc { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.ListOfString, "category", "分类")]
        public List<string> CategoryNames { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.ListOfString, "tag", "标签")]
        public List<string> TagNames { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.String, "title", "标题")]
        public string Title { get; set; }

        private int _visits;

        [BlogPostMetadata(BlogPostMetadataReturnType.Int32, "visit")]
        public int Visits
        {
            get => _visits;

            set => _visits = value;
        }

        [BlogPostMetadata(BlogPostMetadataReturnType.Bool, "Publish")]
        public bool IsPublic { get; set; }

        #endregion

        public string MarkdownContent { get; set; }

        private string _htmlContent;

        public string HtmlContent
        {
            get
            {
                if (string.IsNullOrEmpty(_htmlContent))
                {
                    _htmlContent = GetHtmlContent();
                }

                return _htmlContent;
            }
        }

        private string _excerpt;

        public string Excerpt
        {
            get
            {
                if (string.IsNullOrEmpty(_excerpt))
                {
                    _excerpt = GetExcerptHtml();
                }

                return _excerpt;
            }
        }

        private string _fullUrl;

        public string FullUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_fullUrl) && CreationTimeUtc != default && !string.IsNullOrEmpty(Link))
                {
                    _fullUrl = GetFullUrl(CreationTimeUtc.Year, CreationTimeUtc.Month, Link);
                }

                return _fullUrl;
            }
        }

        private string _gitHubPath;

        public string GitHubPath
        {
            get
            {
                if (string.IsNullOrEmpty(_gitHubPath))
                {
                    _gitHubPath = $"{BlogConstant.PostGitHubPath}{Link}{BlogConstant.PostMarkdownExtension}";
                }

                return _gitHubPath;
            }
        }

        public void AddVisit()
        {
            Interlocked.Increment(ref _visits);
        }

        public void SetDefaults()
        {
            if (CreationTimeUtc == default)
            {
                CreationTimeUtc = DateTime.UtcNow;
            }

            if (LastUpdateTimeUtc == default)
            {
                LastUpdateTimeUtc = DateTime.UtcNow;
            }

            if (Visits == default)
            {
                Visits = 1;
            }

            if (string.IsNullOrEmpty(Title))
            {
                Title = Guid.NewGuid().ToString("D");
            }

            if (string.IsNullOrEmpty(Link))
            {
                throw new PostParseException("Line metadata is required.");
            }
        }

        public static string GetFullUrl(int year, int month, string link)
        {
            return $"https://blog.laobian.me/{year}/{month:D2}/{link}{BlogConstant.PostHtmlExtension}";
        }

        private string GetHtmlContent()
        {
            var html = Markdig.Markdown.ToHtml(MarkdownContent);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var imageNodes = htmlDoc.DocumentNode.Descendants("img");
            foreach (var imageNode in imageNodes)
            {
                if (imageNode.Attributes.Contains("src"))
                {
                    var src = imageNode.Attributes["src"].Value;
                    if (Uri.TryCreate(src, UriKind.Absolute, out var uriResult) &&
                        (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    {
                        // this is Network resources, keep it as it is
                        continue;
                    }

                    if (!Path.IsPathRooted(src))
                    {
                        imageNode.SetAttributeValue("src", $"{BlogConstant.FileRequestPath}/{Path.GetFileName(src)}");
                    }
                }
            }
            return htmlDoc.DocumentNode.OuterHtml;
        }

        private string GetExcerptHtml()
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(HtmlContent);

            var firstNode = htmlDoc.DocumentNode.ChildNodes.FirstOrDefault(_ => string.Equals(_.Name, "p", StringComparison.OrdinalIgnoreCase));
            return firstNode == null ? "<p></p>" : firstNode.OuterHtml;
        }
    }
}
