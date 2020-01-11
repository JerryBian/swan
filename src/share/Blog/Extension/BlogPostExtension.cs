using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Laobian.Share.Blog.Model;
using Laobian.Share.Extension;
using Laobian.Share.Helper;

namespace Laobian.Share.Blog.Extension
{
    public static class BlogPostExtension
    {
        public static string GetMetadataHtml(this BlogPost post)
        {
            var results = new List<string>();
            results.Add(post.GetSimpleMetadataHtml());

            var catHtml = post.GetCategoryHtml();
            if (!string.IsNullOrEmpty(catHtml))
            {
                results.Add(catHtml);
            }

            var tagHtml = post.GetTagHtml();
            if (!string.IsNullOrEmpty(tagHtml))
            {
                results.Add(tagHtml);
            }

            return string.Join(" &middot; ", results);
        }

        public static string GetSimpleMetadataHtml(this BlogPost post)
        {
            var results = new List<string>();
            results.Add(
                $"<i class=\"fas fa-calendar-alt\"></i> <span title=\"{post.PublishTime.ToDateAndTime()}\">发表于 {post.PublishTimeString}</span>");
            results.Add(
                $"<i class=\"fas fa-eye\"></i> <span title=\"{post.AccessCount}\">{post.AccessCountString} 次阅读</span>");

            return string.Join(" &middot; ", results);
        }

        public static string GetCategoryAndTagHtml(this BlogPost post)
        {
            var categoryHtml = post.GetCategoryHtml();
            var tagHtml = post.GetTagHtml();
            if (!string.IsNullOrEmpty(tagHtml))
            {
                return categoryHtml + " &middot; " + tagHtml;
            }

            return categoryHtml;
        }

        public static string GetCategoryHtml(this BlogPost post)
        {
            var results = new List<string>();

            foreach (var blogCategory in post.Categories)
            {
                results.Add($"<a href='{blogCategory.GetLink()}' title='{blogCategory.Name}'>{blogCategory.Name}</a>");
            }

            if (!results.Any())
            {
                return string.Empty;
            }

            return $"<i class=\"fas fa-folder\"></i> <span>{string.Join(", ", results)}</span>";
        }

        public static string GetTagHtml(this BlogPost post)
        {
            var results = new List<string>();

            foreach (var tag in post.Tags)
            {
                results.Add($"<a href='{tag.GetLink()}' title='{tag.Name}'>{tag.Name}</a>");
            }

            if (!results.Any())
            {
                return string.Empty;
            }

            return $"<i class=\"fas fa-tags\"></i> <span>{string.Join(", ", results)}</span>";
        }

        public static void Resolve(
            this BlogPost post,
            List<BlogCategory> allCategories,
            List<BlogTag> allTags)
        {
            SetDefault(post);
            HandleContent(post);
            HandleCategory(post, allCategories);
            HandleTag(post, allTags);

            post.FullUrl =
                $"/{post.PublishTime.Year}/{post.PublishTime.Month:D2}/{post.Link}{Global.Config.Common.HtmlExtension}";
            post.FullUrlWithBase = $"{Global.Config.Blog.BlogAddress}{post.FullUrl}";
        }

        private static void HandleCategory(BlogPost post, List<BlogCategory> allCategories)
        {
            post.Categories.Clear();
            foreach (var categoryName in post.Metadata.Category)
            {
                var category = allCategories?.FirstOrDefault(c => CompareHelper.IgnoreCase(c.Name, categoryName));
                if (category != null)
                {
                    post.Categories.Add(category);
                }
            }
        }

        private static void HandleTag(BlogPost post, List<BlogTag> allTags)
        {
            post.Tags.Clear();
            foreach (var tagName in post.Metadata.Tag)
            {
                var tag = allTags?.FirstOrDefault(c => CompareHelper.IgnoreCase(c.Name, tagName));
                if (tag != null)
                {
                    post.Tags.Add(tag);
                }
            }
        }

        private static void HandleContent(BlogPost post)
        {
            var html = MarkdownHelper.ToHtml(post.ContentMarkdown);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // all images nodes
            var imageNodes = htmlDoc.DocumentNode.Descendants("img").ToList();
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
                            UrlHelper.Combine(Global.Config.Blog.BlogAddress, Global.Config.Blog.FileRequestPath,
                                Path.GetFileName(src)));
                    }
                }
            }

            post.ContentHtml = htmlDoc.DocumentNode.OuterHtml;

            // assign Excerpt
            var excerpt = string.Empty;
            var excerptText = string.Empty;
            var paraNodes =
                htmlDoc.DocumentNode
                    .Descendants()
                    .Where(_ =>
                        CompareHelper.IgnoreCase(_.Name, "p") &&
                        _.Descendants().FirstOrDefault(c => CompareHelper.IgnoreCase(c.Name, "img")) == null).Take(2)
                    .ToList();
            if (paraNodes.Count == 1)
            {
                excerpt += paraNodes[0].OuterHtml;
                excerptText += paraNodes[0].InnerText;
            }

            if (paraNodes.Count == 2)
            {
                excerpt += $"{paraNodes[0].OuterHtml}{paraNodes[1].OuterHtml}";
                excerptText += $"{paraNodes[0].InnerText}{paraNodes[1].InnerText}";
            }

            post.ExcerptPlain = excerptText;
            excerpt += "<p>...</p>";
            post.ExcerptHtml = excerpt;
        }

        private static void SetDefault(BlogPost post)
        {
            if (string.IsNullOrEmpty(post.Metadata.Title))
            {
                post.Metadata.Title = Guid.NewGuid().ToString("N");
            }

            if (string.IsNullOrEmpty(post.Metadata.Link))
            {
                post.Metadata.Link = Guid.NewGuid().ToString("N");
            }
        }
    }
}