using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Blog.HttpClients;
using Laobian.Blog.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.HostedService
{
    public class PostAccessHostedService : BackgroundService
    {
        private readonly IBlogService _blogService;
        private readonly ApiSiteHttpClient _httpClient;
        private readonly ILogger<PostAccessHostedService> _logger;

        public PostAccessHostedService(IBlogService blogService, ApiSiteHttpClient httpClient,
            ILogger<PostAccessHostedService> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _blogService = blogService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessAsync();
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await ProcessAsync();
            await base.StopAsync(cancellationToken);
        }

        private async Task ProcessAsync()
        {
            while (_blogService.TryDequeuePostAccess(out var link))
            {
                try
                {
                    await _httpClient.AddPostAccess(link);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to add new access for post: {link}");
                }
            }
        }
    }
}