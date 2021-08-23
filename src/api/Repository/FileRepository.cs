using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api._2.Source;
using Laobian.Share;
using Laobian.Share.Blog;
using Laobian.Share.Converter;
using Laobian.Share.Logger;
using Laobian.Share.Read;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly IFileSource _fileSource;
        private readonly LaobianApiOption _laobianApiOption;
        private readonly ILogger<FileRepository> _logger;

        public FileRepository(IOptions<LaobianApiOption> apiOption, IFileSource fileSource, ILogger<FileRepository> logger)
        {
            _logger = logger;
            _fileSource = fileSource;
            _laobianApiOption = apiOption.Value;
        }

        public async Task PrepareAsync(CancellationToken cancellationToken = default)
        {
            await _fileSource.PrepareAsync(cancellationToken);
        }

        public async Task SaveAsync(string message)
        {
            await _fileSource.FlushAsync(message);
        }

        public async Task<List<BlogPost>> GetBlogPostsAsync(CancellationToken cancellationToken = default)
        {
            var blogPosts = await _fileSource.ReadBlogPostsAsync(cancellationToken);
            var result = new List<BlogPost>();
            if (blogPosts == null)
            {
                return result;
            }

            foreach (var blogPost in blogPosts)
            {
                var post = JsonUtil.Deserialize<BlogPost>(blogPost);
                result.Add(post);
            }

            return result;
        }

        public async Task<BlogPost> GetBlogPostAsync(string postLink, CancellationToken cancellationToken = default)
        {
            BlogPost result = null;
            var blogPost = await _fileSource.ReadBlogPostAsync(postLink, cancellationToken);
            if (!string.IsNullOrEmpty(blogPost))
            {
                result = JsonUtil.Deserialize<BlogPost>(blogPost);
            }

            return result;
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

            var existingData = await _fileSource.ReadBlogPostAsync(blogPost.Link, cancellationToken);
            if (existingData != null)
            {
                throw new Exception($"Post with link \"{blogPost.Link}\" already exists.");
            }

            blogPost.CreateTime = DateTime.Now;
            blogPost.LastUpdateTime = DateTime.Now;
            await _fileSource.WriteBlogPostAsync(blogPost.CreateTime.Year, blogPost.Link, JsonUtil.Serialize(blogPost),
                cancellationToken);
        }

        public async Task UpdateBlogPostAsync(BlogPost blogPost, CancellationToken cancellationToken = default)
        {
            if (blogPost == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(blogPost.Link))
            {
                throw new Exception("Empty post link provided.");
            }

            var existingData = await _fileSource.ReadBlogPostAsync(blogPost.Link, cancellationToken);
            if (existingData == null)
            {
                throw new Exception($"Post with link \"{blogPost.Link}\" does not exists.");
            }

            var existingPost = JsonUtil.Deserialize<BlogPost>(existingData);
            blogPost.CreateTime = existingPost.CreateTime;
            blogPost.LastUpdateTime = DateTime.Now;
            await _fileSource.WriteBlogPostAsync(blogPost.CreateTime.Year, blogPost.Link, JsonUtil.Serialize(blogPost),
                cancellationToken);
        }

        public async Task<List<BlogAccess>> GetBlogPostAccessAsync(string postLink,
            CancellationToken cancellationToken = default)
        {
            var result = new List<BlogAccess>();
            var blogPostAccess = await _fileSource.ReadBlogPostAccessAsync(postLink, cancellationToken);
            if (!string.IsNullOrEmpty(blogPostAccess))
            {
                result.AddRange(JsonUtil.Deserialize<List<BlogAccess>>(blogPostAccess));
            }

            return result;
        }

        public async Task AddBlogPostAccessAsync(BlogPost blogPost, DateTime date, int count,
            CancellationToken cancellationToken = default)
        {
            var blogPostAccess = await GetBlogPostAccessAsync(blogPost.Link, cancellationToken);
            var access = blogPostAccess.FirstOrDefault(x => x.Date == date);
            if (access == null)
            {
                blogPostAccess.Add(new BlogAccess{Count = count, Date = date});
            }
            else
            {
                access.Count += count;
            }

            await _fileSource.WriteBlogPostAccessAsync(blogPost.CreateTime.Year, blogPost.Link,
                JsonUtil.Serialize(blogPostAccess, converters: new List<JsonConverter> {new DateOnlyConverter()}),
                cancellationToken);
        }

        public async Task<List<BlogTag>> GetBlogTagsAsync(CancellationToken cancellationToken = default)
        {
            var result = new List<BlogTag>();
            var blogTags = await _fileSource.ReadBlogTagsAsync(cancellationToken);
            if (!string.IsNullOrEmpty(blogTags))
            {
                result.AddRange(JsonUtil.Deserialize<List<BlogTag>>(blogTags));
            }

            return result;
        }

        public async Task<BlogTag> GetBlogTagAsync(string tagLink, CancellationToken cancellationToken = default)
        {
            var tags = await GetBlogTagsAsync(cancellationToken);
            return tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, tagLink));
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

            var tags = await GetBlogTagsAsync(cancellationToken);
            var existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, blogTag.Link));
            if (existingTag != null)
            {
                throw new Exception($"Tag with link \"{blogTag.Link}\" already exists.");
            }

            blogTag.LastUpdatedAt = DateTime.Now;
            tags.Add(blogTag);
            await _fileSource.WriteBlogTagsAsync(JsonUtil.Serialize(tags), cancellationToken);
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

            var tags = await GetBlogTagsAsync(cancellationToken);
            var existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, blogTag.Link));
            if (existingTag == null)
            {
                throw new Exception($"Tag with link \"{blogTag.Link}\" does not exists.");
            }

            existingTag.LastUpdatedAt = DateTime.Now;
            existingTag.Description = blogTag.Description;
            existingTag.DisplayName = blogTag.DisplayName;
            await _fileSource.WriteBlogTagsAsync(JsonUtil.Serialize(tags), cancellationToken);
        }

        public async Task DeleteBlogTagAsync(string tagLink, CancellationToken cancellationToken = default)
        {
            var tags = await GetBlogTagsAsync(cancellationToken);
            var existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, tagLink));
            if (existingTag == null)
            {
                throw new Exception($"Tag with link \"{tagLink}\" does not exists.");
            }

            tags.Remove(existingTag);
            await _fileSource.WriteBlogTagsAsync(JsonUtil.Serialize(tags), cancellationToken);
        }

        public async Task<IDictionary<int, List<BookItem>>> GetBookItemsAsync(CancellationToken cancellationToken = default)
        {
            var result = new Dictionary<int, List<BookItem>>();
            var bookItems = await _fileSource.ReadBookItemsAsync(cancellationToken);
            if (bookItems != null)
            {
                foreach (var bookItem in bookItems)
                {
                    result.Add(Convert.ToInt32(bookItem.Key), JsonUtil.Deserialize<List<BookItem>>(bookItem.Value));
                }
            }

            return result;
        }

        public async Task<List<BookItem>> GetBookItemsAsync(int year, CancellationToken cancellationToken = default)
        {
            var bookItems = await _fileSource.ReadBookItemsAsync(year, cancellationToken);
            if (string.IsNullOrEmpty(bookItems))
            {
                return null;
            }

            return JsonUtil.Deserialize<List<BookItem>>(bookItems);
        }

        public async Task AddBookItemAsync(BookItem bookItem, CancellationToken cancellationToken = default)
        {
            var existingBookItems = await GetBookItemsAsync(bookItem.StartTime.Year, cancellationToken);
            if (existingBookItems == null)
            {
                existingBookItems = new List<BookItem>();
            }

            var allBookItems = (await GetBookItemsAsync(cancellationToken)).SelectMany(x => x.Value).ToList();
            if (allBookItems.FirstOrDefault(x => x.Id == bookItem.Id) != null)
            {
                throw new Exception($"BookItem with Id \"{bookItem.Id}\" already exists.");
            }

            if (allBookItems.FirstOrDefault(x => x.BookName == bookItem.BookName) != null)
            {
                _logger.LogWarning($"It appears you already added same book before: {bookItem.BookName}, however it's allowed.");
            }

            bookItem.LastUpdateTime = DateTime.Now;
            existingBookItems.Add(bookItem);
            await _fileSource.WriteBookItemsAsync(bookItem.StartTime.Year, JsonUtil.Serialize(existingBookItems),
                cancellationToken);
        }

        public async Task UpdateBookItemAsync(BookItem bookItem, CancellationToken cancellationToken = default)
        {
            var existingBookItems = await GetBookItemsAsync(bookItem.StartTime.Year, cancellationToken);
            if (existingBookItems == null)
            {
                throw new Exception($"BookItem with Id \"{bookItem.Id}\" not exist.");
            }

            var existingBookItem = existingBookItems.FirstOrDefault(x => x.Id == bookItem.Id);
            if (existingBookItem == null)
            {
                throw new Exception($"BookItem with Id \"{bookItem.Id}\" not exist.");
            }

            bookItem.LastUpdateTime = DateTime.Now;
            existingBookItems.Remove(existingBookItem);
            existingBookItems.Add(bookItem);
            await _fileSource.WriteBookItemsAsync(bookItem.StartTime.Year, JsonUtil.Serialize(existingBookItems.OrderByDescending(x => x.StartTime)),
                cancellationToken);
        }

        public async Task DeleteBookItemAsync(string bookItemId,
            CancellationToken cancellationToken = default)
        {
            var allBookItems = (await GetBookItemsAsync(cancellationToken)).SelectMany(x => x.Value).ToList();
            var bookItem = allBookItems.FirstOrDefault(x => x.Id == bookItemId);
            if (bookItem != null)
            {
                var existingBookItems = await GetBookItemsAsync(bookItem.StartTime.Year, cancellationToken);
                if (existingBookItems != null)
                {
                    var existingBookItem = existingBookItems.FirstOrDefault(x => x.Id == bookItemId);
                    if (existingBookItem != null)
                    {
                        existingBookItems.Remove(existingBookItem);
                        await _fileSource.WriteBookItemsAsync(bookItem.StartTime.Year,
                            JsonUtil.Serialize(existingBookItems.OrderByDescending(x => x.StartTime)),
                            cancellationToken);
                    }
                }
            }
        }

        public async Task<List<LaobianLog>> GetLogsAsync(LaobianSite site, DateTime date, CancellationToken cancellationToken = default)
        {
            var result = new List<LaobianLog>();
            var logs = await _fileSource.ReadLogsAsync(site, date, cancellationToken);
            if (!string.IsNullOrEmpty(logs))
            {
                using var sr = new StringReader(logs);
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    result.Add(JsonUtil.Deserialize<LaobianLog>(line));
                }
            }

            return result;
        }

        public async Task AddLogAsync(LaobianLog log, CancellationToken cancellationToken = default)
        {
            var site = LaobianSite.Api;
            if (Enum.TryParse(log.LoggerName, true, out LaobianSite temp))
            {
                site = temp;
            }

            await _fileSource.AppendLogAsync(site, log.TimeStamp.Date, JsonUtil.Serialize(log), cancellationToken);
        }

        public async Task<string> AddRawFileAsync(string fileName, byte[] content,
            CancellationToken cancellationToken = default)
        {
            return await _fileSource.AddRawFileAsync(fileName, content, cancellationToken);
        }
    }
}
