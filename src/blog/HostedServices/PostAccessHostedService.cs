using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Blog.Service;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.HostedServices;

public class PostAccessHostedService : BackgroundService
{
    private readonly IBlogGrpcService _blogGrpcService;
    private readonly IBlogService _blogService;
    private readonly ILogger<PostAccessHostedService> _logger;

    public PostAccessHostedService(IBlogService blogService, IOptions<BlogOptions> options,
        ILogger<PostAccessHostedService> logger)
    {
        _logger = logger;
        _blogService = blogService;
        _blogGrpcService = GrpcClientHelper.CreateClient<IBlogGrpcService>(options.Value.ApiLocalEndpoint);
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
                var request = new BlogGrpcRequest {Link = link};
                var response = await _blogGrpcService.AddPostAccessAsync(request);
                if (!response.IsOk)
                {
                    _logger.LogError($"Add post access for {link} failed: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add new access for post: {link}");
            }
        }
    }
}