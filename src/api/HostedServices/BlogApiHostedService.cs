using System;
using System.Collections.Generic;
using System.Linq;
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
            await Task.CompletedTask;
        }
    }
}
