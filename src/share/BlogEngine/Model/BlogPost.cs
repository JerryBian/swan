using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using HtmlAgilityPack;
using Humanizer;
using Laobian.Share.Config;
using Laobian.Share.Helper;

namespace Laobian.Share.BlogEngine.Model
{
    public class BlogPost
    {
        private bool _excerptLoaded;

        public BlogPost()
        {
            CategoryNames = new List<string>();
            TagNames = new List<string>();
        }

        #region Post Metadata

        public string Link { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.DateTime, "PublishAfter")]
        public DateTime PublishAfter { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.DateTime, "CreateAt")]
        public DateTime CreationTimeUtc { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.DateTime, "UpdateAt", IsAssignable = false)]
        public DateTime LastUpdateTimeUtc { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.ListOfString, "Category", "分类")]
        public List<string> CategoryNames { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.ListOfString, "Tag", "标签")]
        public List<string> TagNames { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.String, "Title", "标题")]
        public string Title { get; set; }

        private int _visits;

        [BlogPostMetadata(BlogPostMetadataReturnType.Int32, "Visit")]
        public int Visits
        {
            get => _visits;

            set => _visits = value;
        }

        [BlogPostMetadata(BlogPostMetadataReturnType.Bool, "Publish")]
        public bool IsPublic { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.Bool, "PutAtTop")]
        public bool PutAtTop { get; set; }

        [BlogPostMetadata(BlogPostMetadataReturnType.Bool, "IncludeMathJax")]
        public bool IncludeMathJax { get; set; }

        #endregion

        public AppConfig Config { get; set; }

        public bool IsReallyPublic => IsPublic && DateTime.UtcNow > PublishAfter;


        public string MarkdownContent { get; set; }

        private string _createTimeString;

        public string CreateTimeString
        {
            get
            {
                if (string.IsNullOrEmpty(_createTimeString) && CreationTimeUtc != default)
                {
                    _createTimeString = CreationTimeUtc.Humanize();
                }

                return _createTimeString;
            }
        }

        private string _lastUpdateTimeString;

        public string LastUpdateTimeString
        {
            get
            {
                if (string.IsNullOrEmpty(_lastUpdateTimeString) && LastUpdateTimeUtc != default)
                {
                    _lastUpdateTimeString = LastUpdateTimeUtc.Humanize();
                }

                return _lastUpdateTimeString;
            }
        }

        public string VisitString => Visits.ToMetric(decimals: 1);

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
                LoadExcerpt();
                return _excerpt;
            }
        }

        private string _excerptText;

        public string ExcerptText
        {
            get
            {
                LoadExcerpt();
                return _excerptText;
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

        private string _fullUrlWithBaseAddress;

        public string FullUrlWithBaseAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_fullUrlWithBaseAddress) && CreationTimeUtc != default && !string.IsNullOrEmpty(Link))
                {
                    _fullUrlWithBaseAddress = GetFullUrl(CreationTimeUtc.Year, CreationTimeUtc.Month, Link, true);
                }

                return _fullUrlWithBaseAddress;
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

        private string _localFullPath;

        public string LocalFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(_localFullPath))
                {
                    throw new Exception("Local Full Path is not set.");
                }

                return _localFullPath;
            }
            set => _localFullPath = value;
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

        private string GetFullUrl(int year, int month, string link, bool appendBaseAddress = false)
        {
            if (appendBaseAddress)
            {
                return AddressHelper.GetAddress(Config.Blog.BlogAddress, false, year.ToString(), month.ToString("D2"),
                    $"{link}{BlogConstant.PostHtmlExtension}");
            }

            return AddressHelper.GetAddress(false, year.ToString(), month.ToString("D2"),
                $"{link}{BlogConstant.PostHtmlExtension}");
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
                        imageNode.SetAttributeValue("src",
                            AddressHelper.GetAddress(Config.Blog.BlogAddress, false, BlogConstant.FileRequestPath,
                                Path.GetFileName(src)));
                    }
                }
            }
            return htmlDoc.DocumentNode.OuterHtml;
        }

        private void LoadExcerpt()
        {
            if (_excerptLoaded)
            {
                return;
            }

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(HtmlContent);

            var excerpt = string.Empty;
            var excerptText = string.Empty;
            var paraNodes = 
                htmlDoc.DocumentNode
                    .Descendants()
                    .Where(_ => 
                        StringEqualsHelper.IgnoreCase(_.Name, "p") && 
                        _.Descendants().FirstOrDefault(c => StringEqualsHelper.IgnoreCase(c.Name, "img")) == null).Take(2).ToList();
            if (paraNodes.Count == 1)
            {
                excerpt += paraNodes[0].OuterHtml;
                excerptText += paraNodes[0].InnerText;
            }

            if(paraNodes.Count == 2)
            {
                excerpt += $"{paraNodes[0].OuterHtml}{paraNodes[1].OuterHtml}";
                excerptText += $"{paraNodes[0].InnerText}{paraNodes[1].InnerText}";
            }

            _excerptText = excerptText;
            excerpt += "<p>...</p>";
            _excerpt = excerpt;
            _excerptLoaded = true;
        }
    }
}
