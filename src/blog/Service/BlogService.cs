using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Blog.HttpClients;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Site.Blog;
using Laobian.Share.Site.Read;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Service;

public class BlogService : IBlogService
{
    private readonly List<ReadItem> _allBookItems;
    private readonly List<BlogPostRuntime> _allPosts;
    private readonly List<BlogTag> _allTags;
    private readonly IBlogGrpcService _blogGrpcService;
    private readonly IOptions<BlogOptions> _options;
    private readonly ILogger<BlogService> _logger;
    private readonly ConcurrentQueue<string> _postAccessQueue;
    private readonly ManualResetEventSlim _reloadLock;
    private DateTime _lastReloadTime;

    public BlogService(IOptions<BlogOptions> options, ILogger<BlogService> logger)
    {
        _logger = logger;
        BootTime = DateTime.Now;
        _blogGrpcService = GrpcClientHelper.CreateClient<IBlogGrpcService>(options.Value.ApiLocalEndpoint);
        _allTags = new List<BlogTag>();
        _allBookItems = new List<ReadItem>();
        _allPosts = new List<BlogPostRuntime>();
        _postAccessQueue = new ConcurrentQueue<string>();
        _reloadLock = new ManualResetEventSlim(true);
    }

    public DateTime BootTime { get; }

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

    public List<ReadItem> GetBookItems()
    {
        _reloadLock.Wait();
        return _allBookItems;
    }

    public async Task ReloadAsync()
    {
        _reloadLock.Reset();
        try
        {
            var blogRequest = new BlogRequest{ExtractRuntime = true};
            var postsResponse = await _blogGrpcService.GetPostsAsync(blogRequest);
            if (!postsResponse.IsOk)
            {
                _logger.LogError($"Getting all posts failed: {postsResponse.Message}");
                return;
            }

            var tagsResponse = await _blogGrpcService.GetTagsAsync();
            if (!tagsResponse.IsOk)
            {
                _logger.LogError($"Getting all tags failed: {tagsResponse.Message}");
                return;
            }

            var posts = postsResponse.Posts;
            var tags = tagsResponse.Tags;
            //var bookItems = await _httpClient.GetBookItemsAsync();

            _allPosts.Clear();
            _allPosts.AddRange(posts.OrderByDescending(x => x.Raw.PublishTime));

            _allTags.Clear();
            _allTags.AddRange(tags.OrderByDescending(x => x.LastUpdatedAt));

            _allBookItems.Clear();
            //_allBookItems.AddRange(bookItems.OrderByDescending(x => x.StartTime));
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
        _reloadLock.Wait();
        return _lastReloadTime;
    }

    public void EnqueuePostAccess(string link)
    {
        _postAccessQueue.Enqueue(link);
    }

    public bool TryDequeuePostAccess(out string link)
    {
        return _postAccessQueue.TryDequeue(out link);
    }
}