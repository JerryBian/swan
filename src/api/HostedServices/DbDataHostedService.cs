using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Api.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.HostedServices;

public class DbDataHostedService : BackgroundService
{
    private readonly IGitFileService _gitFileService;
    private readonly ILogger<DbDataHostedService> _logger;

    public DbDataHostedService(IGitFileService gitFileService, ILogger<DbDataHostedService> logger)
    {
        _logger = logger;
        _gitFileService = gitFileService;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _gitFileService.PullAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
            {
                try
                {
                    await _gitFileService.PushAsync(":alarm_clock: sever schedule");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{nameof(DbDataHostedService)}.{nameof(ExecuteAsync)} failed.");
                }
            }

            await Task.Delay(TimeSpan.FromMilliseconds(900), stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _gitFileService.PushAsync(":small_red_triangle_down: server stopping");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        await base.StopAsync(cancellationToken);
    }
}