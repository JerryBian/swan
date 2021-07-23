using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Laobian.Api.Repository;
using Laobian.Share.Blog;
using Laobian.Share.Helper;
using Markdig;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Service
{
    public class BlogService : IBlogService
    {
        private readonly ApiConfig _config;
        private readonly IDbRepository _dbRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public BlogService(IOptions<ApiConfig> config, IDbRepository dbRepository, IBlogPostRepository blogPostRepository)
        {
            _config = config.Value;
            _dbRepository = dbRepository;
            _blogPostRepository = blogPostRepository;
        }

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(_blogPostRepository.LoadAsync(cancellationToken),
                _dbRepository.LoadAsync(cancellationToken));
            await AggregateStoreAsync(cancellationToken);
        }

        public async Task PersistentAsync(string message, CancellationToken cancellationToken = default)
        {
            await _dbRepository.PersistentAsync(cancellationToken);
            await LoadAsync(cancellationToken);

            // TODO: notify Blog site
        }

        public async Task<List<BlogPost>> GetAllPostsAsync(bool onlyPublished = true, CancellationToken cancellationToken = default)
        {
            var blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);
            var allPosts = blogPostStore.GetAll();
            if (onlyPublished)
            {
                allPosts = allPosts.Where(x => x.IsPublished).ToList();
            }

            return allPosts;
        }

        public async Task<List<BlogTag>> GetAllTagsAsync(CancellationToken cancellationToken = default)
        {
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            var allBlogs = blogTagStore.GetAll();
            return allBlogs;
        }

        public async Task<BlogPost> GetPostAsync(string postLink, CancellationToken cancellationToken = default)
        {
            var blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);
            return blogPostStore.GetByLink(postLink);
        }

        public async Task<BlogTag> GetTagAsync(string tagLink, CancellationToken cancellationToken = default)
        {
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            return blogTagStore.GetByLink(tagLink);
        }

        public async Task AddBlogTagAsync(BlogTag tag, CancellationToken cancellationToken = default)
        {
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            blogTagStore.Add(tag);
        }

        public async Task UpdateBlogTagAsync(BlogTag tag, CancellationToken cancellationToken = default)
        {
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            blogTagStore.Update(tag);
        }

        public async Task RemoveBlogTagAsync(string tagLink, CancellationToken cancellationToken = default)
        {
            var blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);
            var allPosts = blogPostStore.GetAll();
            allPosts.ForEach(x => x.Tags.RemoveAll(y => StringHelper.EqualIgnoreCase(y.Link, tagLink)));

            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            blogTagStore.RemoveByLink(tagLink);
        }

        public async Task UpdateBlogPostMetadataAsync(BlogPostMetadata metadata, CancellationToken cancellationToken = default)
        {
            var blogMetadataStore = await _dbRepository.GetBlogMetadataStoreAsync(cancellationToken);
            blogMetadataStore.Update(metadata);
        }

        public async Task AddBlogAccessAsync(string postLink, CancellationToken cancellationToken = default)
        {
            var blogAccessStore = await _dbRepository.GetBlogAccessStoreAsync(cancellationToken);
            blogAccessStore.Add(postLink, DateTime.Now, 1);
        }

        private async Task AggregateStoreAsync(CancellationToken cancellationToken)
        {
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            var blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);
            var blogAccessStore = await _dbRepository.GetBlogAccessStoreAsync(cancellationToken);
            var blogMetadataStore = await _dbRepository.GetBlogMetadataStoreAsync(cancellationToken);
            var blogCommentStore = await _dbRepository.GetBlogCommentStoreAsync(cancellationToken);

            foreach (var blogPost in blogPostStore.GetAll())
            {
                var metadata = blogMetadataStore.GetByLink(blogPost.Link);
                if (metadata == null)
                {
                    metadata = new BlogPostMetadata { Link = blogPost.Link};
                    metadata.SetDefault();
                    blogMetadataStore.Add(metadata);
                }

                blogPost.Metadata = metadata;
                var access = blogAccessStore.GetByLink(blogPost.Link);
                if (access != null)
                {
                    blogPost.Accesses.AddRange(access);
                    blogPost.AccessCount = access.Sum(x => x.Count);
                    blogPost.AccessCountString = blogPost.AccessCount.ToString("N");
                }
                else
                {
                    blogPost.AccessCountString = "0";
                }

                foreach (var metadataTag in metadata.Tags)
                {
                    var tag = blogTagStore.GetByLink(metadataTag);
                    if (tag != null)
                    {
                        blogPost.Tags.Add(tag);
                    }
                }

                var comments = blogCommentStore.GetByLink(blogPost.Link);
                if (comments != null)
                {
                    blogPost.Comments.AddRange(comments);
                    blogPost.CommentCount = blogPost.Comments.Count;
                    blogPost.CommentCountString = comments.Count.ToString("N");
                }
                else
                {
                    blogPost.CommentCountString = "0";
                }

                blogPost.PublishTimeString = blogPost.Metadata.PublishTime.ToString("yyyy-MM-dd HH:mm:ss");
                blogPost.FullPath = $"{blogPost.Metadata.PublishTime.Year:D4}/{blogPost.Metadata.PublishTime.Month:D2}/{blogPost.Metadata.Link}.html";

                NormalizeBlogPost(blogPost);
            }
        }

        private void SetPostThumbnail(BlogPost post, string url)
        {
            if (string.IsNullOrEmpty(post.Thumbnail) && !string.IsNullOrEmpty(url))
            {
                post.Thumbnail = url;
            }
        }

        private void NormalizeBlogPost(BlogPost post)
        {
            if (string.IsNullOrEmpty(post.MdContent))
            {
                post.MdContent = "Post content is empty.";
            }

            var html = Markdown.ToHtml(post.MdContent);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // all images nodes
            const string fileFolder = "file";
            var imageNodes = htmlDoc.DocumentNode.Descendants("img").ToList();
            foreach (var imageNode in imageNodes)
            {
                if (imageNode.Attributes.Contains("src"))
                {
                    var parentNode = imageNode.ParentNode;
                    parentNode?.AddClass("text-center");

                    var src = imageNode.Attributes["src"].Value;
                    if (string.IsNullOrEmpty(src))
                    {
                        continue;
                    }

                    if (Uri.TryCreate(src, UriKind.Absolute, out var uriResult) &&
                        (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    {
                        // this is Network resources, keep it as it is
                        SetPostThumbnail(post, src);
                        continue;
                    }

                    if (!Path.IsPathRooted(src))
                    {
                        var index = src.IndexOf(fileFolder, StringComparison.InvariantCultureIgnoreCase);
                        if (index >= 0)
                        {
                            var subPath = src.Substring(index).Replace("\\", "/");
                            var fullSrc = $"/{subPath}";
                            imageNode.SetAttributeValue("src", fullSrc);
                            SetPostThumbnail(post, fullSrc);
                        }
                    }
                }
            }

            post.HtmlContent = htmlDoc.DocumentNode.OuterHtml;

            // assign Excerpt
            if (!string.IsNullOrEmpty(post.Metadata.Excerpt))
            {
                post.ExcerptHtml = Markdown.ToHtml(post.Metadata.Excerpt);
            }
            else
            {
                var excerpt = string.Empty;
                var excerptText = string.Empty;
                var paraNodes =
                    htmlDoc.DocumentNode
                        .Descendants()
                        .Where(_ =>
                            StringHelper.EqualIgnoreCase(_.Name, "p") &&
                            _.Descendants().FirstOrDefault(c => StringHelper.EqualIgnoreCase(c.Name, "img")) == null).Take(2)
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

                //post.ExcerptPlain = excerptText;
                post.ExcerptHtml = excerpt;
            }

            
        }
    }
}
