
using Swan.Core.Store;

namespace Swan.HostedServices
{
    public class GitStoreHostedService : BackgroundService
    {
        private readonly ISwanStore _swanStore;

        public GitStoreHostedService(ISwanStore swanStore)
        {
            _swanStore = swanStore;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _swanStore.LoadAsync();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
