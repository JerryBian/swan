using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Blog.Service;
using Microsoft.Extensions.Hosting;

namespace Laobian.Blog.HostedService;

public class BlogHostedService : BackgroundService
{
    private readonly IBlogService _blogService;

    public BlogHostedService(IBlogService blogService)
    {
        _blogService = blogService;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _blogService.ReloadAsync();
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (DateTime.Now.Hour == 6 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
            {
                await _blogService.ReloadAsync();
            }
            else
            {
                await Task.Delay(300, stoppingToken);
            }
        }
    }
}