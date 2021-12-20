using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    private readonly List<BlogPostRuntime> _allPosts;
    private readonly List<ReadItemRuntime> _allReadItems;
    private readonly List<BlogTag> _allTags;
    private readonly IBlogGrpcService _blogGrpcService;
    private readonly ILogger<BlogService> _logger;
    private readonly ConcurrentQueue<string> _postAccessQueue;
    private readonly IReadGrpcService _readGrpcService;
    private readonly ManualResetEventSlim _reloadLock;
    private DateTime _lastReloadTime;

    public BlogService(IOptions<BlogOptions> options, ILogger<BlogService> logger)
    {
        _logger = logger;
        BootTime = DateTime.Now;
        _blogGrpcService = GrpcClientHelper.CreateClient<IBlogGrpcService>(options.Value.ApiLocalEndpoint);
        _readGrpcService = GrpcClientHelper.CreateClient<IReadGrpcService>(options.Value.ApiLocalEndpoint);
        _allTags = new List<BlogTag>();
        _allReadItems = new List<ReadItemRuntime>();
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

    public List<ReadItemRuntime> GetReadItems()
    {
        _reloadLock.Wait();
        return _allReadItems;
    }

    public async Task ReloadAsync()
    {
        _reloadLock.Reset();
        try
        {
            var blogRequest = new BlogGrpcRequest {ExtractRuntime = true};
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

            var readRequest = new ReadGrpcRequest {ExtractRuntime = true};
            var readResponse = await _readGrpcService.GetReadItemsAsync(readRequest);
            if (!readResponse.IsOk)
            {
                _logger.LogError($"Getting all read items failed: {tagsResponse.Message}");
                return;
            }

            var posts = postsResponse.Posts ?? new List<BlogPostRuntime>();
            var tags = tagsResponse.Tags ?? new List<BlogTag>();
            var readItems = readResponse.ReadItems ?? new List<ReadItemRuntime>();

            _allPosts.Clear();
            _allPosts.AddRange(posts.OrderByDescending(x => x.Raw.PublishTime));

            _allTags.Clear();
            _allTags.AddRange(tags.OrderByDescending(x => x.LastUpdatedAt));

            _allReadItems.Clear();
            _allReadItems.AddRange(readItems.OrderByDescending(x => x.Raw.StartTime));
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