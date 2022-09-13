using Laobian.Lib.Worker;

namespace Laobian.Web.HostedServices
{
    public class BlogPostHostedService : BackgroundService
    {
        private readonly IBlogPostAccessWorker _blogPostAccessWorker;

        public BlogPostHostedService(IBlogPostAccessWorker blogPostAccessWorker)
        {
            _blogPostAccessWorker = blogPostAccessWorker;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _blogPostAccessWorker.ProcessAsync();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _blogPostAccessWorker.StopAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
