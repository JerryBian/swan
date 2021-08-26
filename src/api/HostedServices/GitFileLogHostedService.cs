using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Logger;
using Laobian.Share.Site;
using Microsoft.Extensions.Hosting;

namespace Laobian.Api.HostedServices
{
    public class GitFileLogHostedService : BackgroundService
    {
        private readonly IFileRepository _fileRepository;
        private readonly ILaobianLogQueue _logQueue;

        public GitFileLogHostedService(ILaobianLogQueue logQueue, IFileRepository fileRepository)
        {
            _logQueue = logQueue;
            _fileRepository = fileRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessLogsAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromMilliseconds(300), stoppingToken);
            }
        }

        private async Task ProcessLogsAsync(CancellationToken stoppingToken)
        {
            while (_logQueue.TryDequeue(out var log))
            {
                try
                {
                    log.LoggerName = string.IsNullOrEmpty(log.LoggerName) ? LaobianSite.Api.ToString() : log.LoggerName;
                    await _fileRepository.AddLogAsync(log, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await ProcessLogsAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}