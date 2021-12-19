using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Api.Source;
using Laobian.Share;
using Laobian.Share.Site.Blog;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Service
{
    public class BlogFileService : IBlogFileService
    {
        private readonly IBlogFileSource _blogFileSource;
        private readonly ILogger<BlogFileService> _logger;

        public BlogFileService(IBlogFileSource blogFileSource, ILogger<BlogFileService> logger)
        {
            _logger = logger;
            _blogFileSource = blogFileSource;
        }

        #region Blog

        public async Task<List<BlogPost>> GetBlogPostsAsync(CancellationToken cancellationToken = default)
        {
            var posts = new List<BlogPost>();
            foreach (var item in await _blogFileSource.SearchAsync("*.json", Constants.AssetDbBlogPostFolder, cancellationToken))
            {
                var postJson = await _blogFileSource.ReadAsync(item, cancellationToken);
                if (string.IsNullOrEmpty(postJson))
                {
                    _logger.LogWarning($"Post file is empty: {item}");
                    continue;
                }

                var post = JsonUtil.Deserialize<BlogPost>(postJson);
                posts.Add(post);
            }

            return posts;
        }

        public async Task<BlogPost> GetBlogPostAsync(string postLink, CancellationToken cancellationToken = default)
        {
            postLink = postLink.ToLowerInvariant();
            var postFile = (await _blogFileSource.SearchAsync($"{postLink}.json", Constants.AssetDbBlogPostFolder,
                cancellationToken)).FirstOrDefault();
            if (postFile != null)
            {
                var postJson = await _blogFileSource.ReadAsync(postFile, cancellationToken);
                if (string.IsNullOrEmpty(postJson))
                {
                    _logger.LogWarning($"Post file is empty: {postFile}");
                    return null;
                }

                return JsonUtil.Deserialize<BlogPost>(postJson);
            }

            return null;
        }

        public async Task AddBlogPostAsync(BlogPost blogPost, CancellationToken cancellationToken = default)
        {
            if (blogPost == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(blogPost.Link))
            {
                throw new Exception("Empty post link provided.");
            }

            blogPost.Link = blogPost.Link.ToLowerInvariant();
            var existingData = await GetBlogPostAsync(blogPost.Link, cancellationToken);
            if (existingData != null)
            {
                throw new Exception($"Post with link \"{blogPost.Link}\" already exists.");
            }

            blogPost.CreateTime = DateTime.Now;
            blogPost.LastUpdateTime = DateTime.Now;
            await _blogFileSource.WriteAsync(
                Path.Combine(Constants.AssetDbBlogPostFolder, blogPost.CreateTime.Year.ToString("D4"), $"{blogPost.Link}.json"),
                JsonUtil.Serialize(blogPost, true),
                cancellationToken);
        }

        public async Task UpdateBlogPostAsync(BlogPost blogPost, string originalPostLink, CancellationToken cancellationToken = default)
        {
            if (blogPost == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(blogPost.Link))
            {
                throw new Exception("Empty post link provided.");
            }

            blogPost.Link = blogPost.Link.ToLowerInvariant();
            var existingPost = await GetBlogPostAsync(blogPost.Link, cancellationToken);
            if (existingPost == null)
            {
                throw new Exception($"Post with link \"{blogPost.Link}\" not exists.");
            }

            blogPost.CreateTime = existingPost.CreateTime;
            blogPost.LastUpdateTime = DateTime.Now;

            // The post link got changed
            var postLinkChanged = !StringUtil.EqualsIgnoreCase(blogPost.Link, originalPostLink);
            if (postLinkChanged)
            {
                var existingData =
                    (await _blogFileSource.SearchAsync($"{blogPost.Link}.json", cancellationToken: cancellationToken))
                    .FirstOrDefault();
                if (!string.IsNullOrEmpty(existingData))
                {
                    throw new Exception($"Post with link \"{blogPost.Link}\" already exists.");
                }
            }

            await _blogFileSource.WriteAsync(Path.Combine(Constants.AssetDbBlogPostFolder, blogPost.CreateTime.Year.ToString("D4"), $"{blogPost.Link}.json"),
                JsonUtil.Serialize(blogPost, true),
                cancellationToken);

            if (postLinkChanged)
            {
                var oldAccessFile = Path.Combine(Constants.AssetDbBlogAccessFolder, blogPost.CreateTime.Year.ToString("D4"), $"{originalPostLink}.json");
                if (await _blogFileSource.FileExistsAsync(oldAccessFile, cancellationToken))
                {
                    var newAccessFile = Path.Combine(Constants.AssetDbBlogAccessFolder, blogPost.CreateTime.Year.ToString("D4"), $"{blogPost.Link}.json");
                    await _blogFileSource.RenameAsync(oldAccessFile, newAccessFile, cancellationToken);
                }

                await DeleteBlogPostAsync(originalPostLink, cancellationToken);
            }
        }

        public async Task DeleteBlogPostAsync(string postLink, CancellationToken cancellationToken = default)
        {
            var postFile = (await _blogFileSource.SearchAsync($"{postLink.ToLowerInvariant()}.json",
                cancellationToken: cancellationToken)).FirstOrDefault();
            if (!string.IsNullOrEmpty(postFile))
            {
                await _blogFileSource.DeleteAsync(postFile, cancellationToken);
            }
        }

        public async Task<List<BlogAccess>> GetBlogPostAccessAsync(string postLink, CancellationToken cancellationToken = default)
        {
            var blogAccesses = new List<BlogAccess>();
            var blogPost = await GetBlogPostAsync(postLink, cancellationToken);
            if (blogPost == null)
            {
                throw new Exception($"Blog post not exists: {postLink}");
            }

            var accessFile = Path.Combine(Constants.AssetDbBlogAccessFolder, blogPost.CreateTime.Year.ToString("D4"),
                $"{postLink.ToLowerInvariant()}.json");
            if(await _blogFileSource.FileExistsAsync(accessFile, cancellationToken))
            {
                var accessJson = await _blogFileSource.ReadAsync(accessFile, cancellationToken);
                if (!string.IsNullOrEmpty(accessJson))
                {
                    blogAccesses.AddRange(JsonUtil.Deserialize<IEnumerable<BlogAccess>>(accessJson));
                }
            }

            return blogAccesses;
        }

        public async Task AddBlogPostAccessAsync(string postLink, DateTime date, int count, CancellationToken cancellationToken = default)
        {
            var blogPost = await GetBlogPostAsync(postLink, cancellationToken);
            if (blogPost == null)
            {
                throw new Exception($"Blog post not exists: {postLink}");
            }

            var accessFile = Path.Combine(Constants.AssetDbBlogAccessFolder, blogPost.CreateTime.Year.ToString("D4"),
                $"{postLink.ToLowerInvariant()}.json");
            var existingData = await GetBlogPostAccessAsync(postLink, cancellationToken);
            var dateAccess = existingData.FirstOrDefault(x => x.Date.Date == date.Date);
            if (dateAccess == null)
            {
                dateAccess = new BlogAccess {Count = 0, Date = date};
                existingData.Add(dateAccess);
            }

            dateAccess.Count += count;
            await _blogFileSource.WriteAsync(accessFile,
                JsonUtil.Serialize(existingData.OrderByDescending(x => x.Date)), cancellationToken);
        }

        public async Task<List<BlogTag>> GetBlogTagsAsync(CancellationToken cancellationToken = default)
        {
            var blogTags = new List<BlogTag>();
            const string tagFile = "tag.json";
            if (await _blogFileSource.FileExistsAsync(tagFile, cancellationToken))
            {
                var tagJson = await _blogFileSource.ReadAsync(tagFile, cancellationToken);
                if (!string.IsNullOrEmpty(tagJson))
                {
                    blogTags.AddRange(JsonUtil.Deserialize<IEnumerable<BlogTag>>(tagJson));
                }
            }

            return blogTags;
        }

        public async Task<BlogTag> GetBlogTagAsync(string id, CancellationToken cancellationToken = default)
        {
            var blogTags = await GetBlogTagsAsync(cancellationToken);
            return blogTags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(id, x.Id));
        }

        public async Task AddBlogTagAsync(BlogTag blogTag, CancellationToken cancellationToken = default)
        {
            if (blogTag == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(blogTag.Link))
            {
                throw new Exception("Empty tag link provided.");
            }

            if (string.IsNullOrEmpty(blogTag.DisplayName))
            {
                throw new Exception("Empty tag display name provided.");
            }

            var tags = await GetBlogTagsAsync(cancellationToken);
            var existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, blogTag.Link));
            if (existingTag != null)
            {
                throw new Exception($"Tag with link \"{blogTag.Link}\" already exists.");
            }

            existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.DisplayName, blogTag.DisplayName));
            if (existingTag != null)
            {
                throw new Exception($"Tag with DisplayName \"{blogTag.DisplayName}\" already exists.");
            }

            blogTag.Id ??= StringUtil.GenerateRandom();
            blogTag.LastUpdatedAt = DateTime.Now;
            tags.Add(blogTag);
            await _blogFileSource.WriteAsync("tag.json", JsonUtil.Serialize(tags.OrderByDescending(x => x.LastUpdatedAt), true), cancellationToken);
        }

        public async Task UpdateBlogTagAsync(BlogTag blogTag, CancellationToken cancellationToken = default)
        {
            if (blogTag == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(blogTag.Link))
            {
                throw new Exception("Empty tag link provided.");
            }

            if (string.IsNullOrEmpty(blogTag.DisplayName))
            {
                throw new Exception("Empty tag display name provided.");
            }

            var tags = await GetBlogTagsAsync(cancellationToken);
            var existingTag =
                tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, blogTag.Link) && x.Id != blogTag.Id);
            if (existingTag != null)
            {
                throw new Exception($"Tag with link \"{blogTag.Link}\" already exists.");
            }

            existingTag = tags.FirstOrDefault(x =>
                StringUtil.EqualsIgnoreCase(x.DisplayName, blogTag.DisplayName) && x.Id != blogTag.Id);
            if (existingTag != null)
            {
                throw new Exception($"Tag with DisplayName \"{blogTag.DisplayName}\" already exists.");
            }

            existingTag = tags.FirstOrDefault(x => x.Id == blogTag.Id);
            if (existingTag == null)
            {
                throw new Exception($"Tag with id \"{blogTag.Id}\" does not exists.");
            }

            existingTag.LastUpdatedAt = DateTime.Now;
            existingTag.Description = blogTag.Description;
            existingTag.DisplayName = blogTag.DisplayName;
            existingTag.Link = blogTag.Link;
            await _blogFileSource.WriteAsync("tag.json", JsonUtil.Serialize(tags.OrderByDescending(x => x.LastUpdatedAt), true), cancellationToken);
        }

        public async Task DeleteBlogTagAsync(string id, CancellationToken cancellationToken = default)
        {
            var tags = await GetBlogTagsAsync(cancellationToken);
            var existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(id, x.Id));
            if (existingTag == null)
            {
                throw new Exception($"Tag with id \"{id}\" does not exists.");
            }

            tags.Remove(existingTag);
            await _blogFileSource.WriteAsync("tag.json", JsonUtil.Serialize(tags.OrderByDescending(x => x.LastUpdatedAt), true), cancellationToken);
        }

#endregion
    }
}
