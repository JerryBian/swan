using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Laobian.Share.Blog.Model;
using Laobian.Share.Config;
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
            results.Add($"<i class=\"fas fa-calendar-alt\"></i> <span>发表于 {post.PublishTimeString}</span>");
            results.Add($"<i class=\"fas fa-eye\"></i> <span>{post.AccessCountString} 次访问</span>");

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

        public static void Resolve(this BlogPost post, AppConfig appConfig, List<BlogPost> allPosts, List<BlogCategory> allCategories, List<BlogTag> allTags)
        {
            SetDefault(post);
            HandleContent(post, appConfig);
            HandleCategory(post, allCategories);
            HandleTag(post, allTags);

            post.FullUrl =
                $"/{post.PublishTime.Year}/{post.PublishTime.Month:D2}/{post.Link}{appConfig.Common.HtmlExtension}";
            post.FullUrlWithBase = $"{appConfig.Blog.BlogAddress}{post.FullUrl}";
        }

        private static void HandleCategory(BlogPost post, List<BlogCategory> allCategories)
        {
            post.Categories.Clear();
            foreach (var categoryName in post.Raw.Category)
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
            foreach (var tagName in post.Raw.Tag)
            {
                var tag = allTags?.FirstOrDefault(c => CompareHelper.IgnoreCase(c.Name, tagName));
                if (tag != null)
                {
                    post.Tags.Add(tag);
                }
            }
        }

        private static void HandleContent(BlogPost post, AppConfig config)
        {
            var html = MarkdownHelper.ToHtml(post.Raw.Markdown);
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
                        imageNode.SetAttributeValue("src", $"{config.Blog.BlogAddress}{config.Blog.FileRequestPath}/{Path.GetFileName(src)}");
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
                        _.Descendants().FirstOrDefault(c => CompareHelper.IgnoreCase(c.Name, "img")) == null).Take(2).ToList();
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
            if (post.Raw.CreateTime == null)
            {
                post.Raw.CreateTime = DateTime.Now;
            }

            if (post.Raw.LastUpdateTime == null)
            {
                post.Raw.LastUpdateTime = DateTime.Now;
            }

            if (string.IsNullOrEmpty(post.Raw.Title))
            {
                post.Raw.Title = Guid.NewGuid().ToString("N");
            }

            if (string.IsNullOrEmpty(post.Raw.Link))
            {
                post.Raw.Link = Guid.NewGuid().ToString("N");
            }
        }
    }
}