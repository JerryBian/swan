using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Service;
using Microsoft.Extensions.Hosting;

namespace Laobian.Api.HostedServices
{
    public class BlogApiHostedService : BackgroundService
    {
        private readonly IBlogService _blogService;

        public BlogApiHostedService(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _blogService.LoadAsync(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                {
                    await _blogService.PersistentAsync(":alarm_clock: triggered by server automatically",
                        stoppingToken);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(300), stoppingToken);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _blogService.PersistentAsync(":small_red_triangle_down: triggered by server stopping",
                CancellationToken.None);
            await base.StopAsync(cancellationToken);
        }
    }
}