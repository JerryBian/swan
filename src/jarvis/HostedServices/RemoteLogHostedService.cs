using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Jarvis.HttpClients;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Logger;
using Laobian.Share.Site;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Laobian.Jarvis.HostedServices;

public class RemoteLogHostedService : BackgroundService
{
    private readonly ILogGrpcService _logGrpcService;
    private readonly ILaobianLogQueue _logQueue;

    public RemoteLogHostedService(ILaobianLogQueue logQueue, IOptions<JarvisOptions> options)
    {
        _logQueue = logQueue;
        _logGrpcService = GrpcClientHelper.CreateClient<ILogGrpcService>(options.Value.ApiLocalEndpoint);
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
                var request = new LogGrpcRequest {Logger = LaobianSite.Jarvis.ToString(), Logs = logs};
                var response = await _logGrpcService.AddLogsAsync(request);
                if (!response.IsOk)
                {
                    Console.WriteLine($"Send logs failed: {response.Message}");
                }
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