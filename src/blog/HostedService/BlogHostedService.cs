using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Blog.Data;
using Microsoft.Extensions.Hosting;

namespace Laobian.Blog.HostedService
{
    public class BlogHostedService : BackgroundService
    {
        private readonly ISystemData _systemData;

        public BlogHostedService(ISystemData systemData)
        {
            _systemData = systemData;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _systemData.LoadAsync();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.Hour == 6 && DateTime.Now.Minute == 0)
                {
                    await _systemData.LoadAsync();
                }
                else
                {
                    await Task.Delay(300, stoppingToken);
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}