using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Logger;
using Laobian.Share.Site;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.HostedService;

public class RemoteLogHostedService : BackgroundService
{
    private readonly ILaobianLogQueue _logQueue;
    private readonly AdminOptions _options;

    public RemoteLogHostedService(ILaobianLogQueue logQueue, IOptions<AdminOptions> options)
    {
        _logQueue = logQueue;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SendLogsAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await SendLogsAsync();
        await base.StopAsync(cancellationToken);
    }

    private async Task SendLogsAsync()
    {
        var logs = new List<LaobianLog>();
        while (_logQueue.TryDequeue(out var log))
        {
            logs.Add(log);
        }

        if (logs.Any())
        {
            try
            {
                var client = GrpcClientHelper.CreateClient<ILogGrpcService>(_options.ApiLocalEndpoint);
                var request = new LogGrpcRequest {Logger = LaobianSite.Admin.ToString(), Logs = logs};
                await client.AddLogsAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sent logs failed. {ex}");
                foreach (var laobianLog in logs)
                {
                    Console.WriteLine(laobianLog);
                }
            }
        }
    }
}