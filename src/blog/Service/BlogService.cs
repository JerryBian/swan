using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Blog.HttpClients;
using Laobian.Share.Blog;
using Laobian.Share.Read;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Service
{
    public class BlogService : IBlogService
    {
        private readonly List<BookItem> _allBookItems;
        private readonly List<BlogPostRuntime> _allPosts;
        private readonly List<BlogTag> _allTags;
        private readonly ApiSiteHttpClient _httpClient;
        private readonly ILogger<BlogService> _logger;
        private readonly ManualResetEventSlim _reloadLock;
        private DateTime _lastReloadTime;

        public BlogService(ApiSiteHttpClient httpClient, ILogger<BlogService> logger)
        {
            _logger = logger;
            BootTime = DateTime.Now;
            _httpClient = httpClient;
            _allTags = new List<BlogTag>();
            _allBookItems = new List<BookItem>();
            _allPosts = new List<BlogPostRuntime>();
            RuntimeVersion = RuntimeInformation.FrameworkDescription;
            _reloadLock = new ManualResetEventSlim(true);
        }

        public string AppVersion
        {
            get
            {
                var ver = Assembly.GetEntryAssembly()?.GetName().Version;
                if (ver == null)
                {
                    return "1.0";
                }

                return $"{ver.Major}.{ver.Minor}";
            }
        }

        public DateTime BootTime { get; }

        public string RuntimeVersion { get; }

        public List<BlogPostRuntime> GetAllPosts()
        {
            _reloadLock.Wait();
            return _allPosts;
        }

        public List<BlogTag> GetAllTags()
        {
            _reloadLock.Wait();
            return _allTags;
        }

        public List<BookItem> GetBookItems()
        {
            _reloadLock.Wait();
            return _allBookItems;
        }

        public async Task ReloadAsync()
        {
            _reloadLock.Reset();
            try
            {
                var posts = await _httpClient.GetPostsAsync();
                var tags = await _httpClient.GetTagsAsync();
                var bookItems = await _httpClient.GetBookItemsAsync();

                _allPosts.Clear();
                _allPosts.AddRange(posts.OrderByDescending(x => x.Raw.PublishTime));

                _allTags.Clear();
                _allTags.AddRange(tags.OrderByDescending(x => x.LastUpdatedAt));

                _allBookItems.Clear();
                _allBookItems.AddRange(bookItems.OrderByDescending(x => x.StartTime));
                _lastReloadTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reload blog data failed.");
            }
            finally
            {
                _reloadLock.Set();
            }
        }

        public DateTime GetLastReloadTime()
        {
            return _lastReloadTime;
        }
    }
}