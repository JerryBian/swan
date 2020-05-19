using System;
using System.Collections.Concurrent;
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
        private static void SetMetadataHtml(BlogPost post)
        {
            var results = new List<string>();
            results.Add(
                $"<span title=\"{post.PublishTime.ToDateAndTime()}\">发表于 {post.PublishTimeString}</span>");
            results.Add(
                $"<span title=\"{post.AccessCount}\">{post.AccessCountString} 次阅读</span>");

            post.MetadataHtml = string.Join(" &middot; ", results);
        }

        private static void SetCategoryAndTagHtml(BlogPost post)
        {
            var categoryHtml = GetCategoryHtml(post);
            var tagHtml = GetTagHtml(post);
            if (!string.IsNullOrEmpty(tagHtml))
            {
                post.CategoryAndTagHtml = categoryHtml + " &middot; " + tagHtml;
            }
            else
            {
                post.CategoryAndTagHtml = categoryHtml;
            }
        }

        private static string GetCategoryHtml(BlogPost post)
        {
            var results = new ConcurrentBag<string>();
            foreach (var blogCategory in post.Categories)
            {
                results.Add($"<a href='{blogCategory.GetLink()}' title='{blogCategory.Name}'>{blogCategory.Name}</a>");
            }

            if (!results.Any())
            {
                return string.Empty;
            }

            return $"分类：<span>{string.Join(", ", results)}</span>";
        }

        private static string GetTagHtml(BlogPost post)
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

            return $"标签：<span>{string.Join(", ", results)}</span>";
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
            post.FullUrlWithBase = UrlHelper.Combine(Global.Config.Blog.BlogAddress, post.FullUrl);

            SetCategoryAndTagHtml(post);
            SetMetadataHtml(post);
            SetHeadDescription(post);
        }

        private static void SetHeadDescription(BlogPost post)
        {
            var maxLength = 145 - Global.Config.Blog.Description.Length;
            var description = post.ExcerptPlain.Substring(0,
                post.ExcerptPlain.Length < maxLength ? post.ExcerptPlain.Length : maxLength);
            description += "...";
            post.HeadDescription = description;
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
                    var parentNode = imageNode.ParentNode;
                    parentNode?.AddClass("text-center");

                    var src = imageNode.Attributes["src"].Value;
                    if (Uri.TryCreate(src, UriKind.Absolute, out var uriResult) &&
                        (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    {
                        // this is Network resources, keep it as it is
                        continue;
                    }

                    if (!Path.IsPathRooted(src))
                    {
                        var fileFolderName = Global.Config.Blog.FileGitPath.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
                        var parts = new List<string>();
                        var found = false;
                        foreach (var item in src.Split('/', StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (found)
                            {
                                parts.Add(item);
                                continue;
                            }

                            if (item == fileFolderName)
                            {
                                found = true;
                            }
                        }

                        parts.Insert(0, Global.Config.Blog.FileRequestPath);
                        imageNode.SetAttributeValue("src",
                            UrlHelper.Combine(Global.Config.Blog.StaticAddress, parts.ToArray()));
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
                excerpt += $"<p>{paraNodes[0].InnerText}</p>";
                excerptText += paraNodes[0].InnerText;
            }

            if (paraNodes.Count == 2)
            {
                excerpt += $"<p>{paraNodes[0].InnerText}</p><p>{paraNodes[1].InnerText}</p>";
                excerptText += $"{paraNodes[0].InnerText}{paraNodes[1].InnerText}";
            }

            post.ExcerptPlain = excerptText;
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