using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Admin.HttpService;
using Laobian.Share.Logger;
using Microsoft.Extensions.Hosting;

namespace Laobian.Admin.HostedService
{
    public class LogHostedService : BackgroundService
    {
        private readonly ApiHttpService _apiHttpService;
        private readonly ILaobianLogQueue _logQueue;

        public LogHostedService(ILaobianLogQueue logQueue, ApiHttpService apiHttpService)
        {
            _logQueue = logQueue;
            _apiHttpService = apiHttpService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
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
                        await _apiHttpService.SendLogsAsync(logs);
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
                else
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
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
                    await _apiHttpService.SendLogsAsync(logs);
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

            await base.StopAsync(cancellationToken);
        }
    }
}