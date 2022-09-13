using Laobian.Lib.Extension;
using Laobian.Lib.Service;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Laobian.Lib.Worker
{
    public class BlogPostAccessWorker : IBlogPostAccessWorker
    {
        private readonly ConcurrentQueue<string> _posts;
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogPostAccessWorker> _logger;
        private readonly CancellationTokenSource _cts;

        public BlogPostAccessWorker(IBlogService blogService, ILogger<BlogPostAccessWorker> logger)
        {
            _logger = logger;
            _blogService = blogService;
            _posts = new ConcurrentQueue<string>();
            _cts = new CancellationTokenSource();
        }

        public async void Add(string id)
        {
            if (_cts.IsCancellationRequested)
            {
                _ = await _blogService.AddPostAccessAsync(id, 1);
                return;
            }

            _posts.Enqueue(id);
        }

        public async Task ProcessAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                await ProcessInternalAsync();
                await Task.Delay(TimeSpan.FromHours(12)).OkForCancel();
            }
        }

        public async Task StopAsync()
        {
            _cts.Cancel();
            await ProcessInternalAsync();
        }

        private async Task ProcessInternalAsync()
        {
            try
            {
                Dictionary<string, int> dict = new();
                while (_posts.TryDequeue(out string id))
                {
                    if (!dict.ContainsKey(id))
                    {
                        dict.Add(id, 0);
                    }

                    dict[id]++;
                }

                foreach (KeyValuePair<string, int> item in dict)
                {
                    _ = await _blogService.AddPostAccessAsync(item.Key, item.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blog post access worker processing failed.");
            }
        }
    }
}
