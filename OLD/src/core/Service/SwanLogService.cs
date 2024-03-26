using GitStoreDotnet;
using Swan.Core.Helper;
using Swan.Core.Model;

namespace Swan.Core.Service
{
    internal class SwanLogService : ISwanLogService
    {
        private readonly IGitStore _gitStore;
        private readonly SemaphoreSlim _semaphoreSlim;

        public SwanLogService(IGitStore gitStore)
        {
            _gitStore = gitStore;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task AddAsync(SwanLog log)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var logs = JsonHelper.Deserialize<List<SwanLog>>(await _gitStore.GetTextAsync(log.GetGitStorePath()));
                logs ??= [];
                logs.Add(log);

                logs.RemoveAll(x => DateTime.Now.Date - x.CreatedAt.Date > TimeSpan.FromDays(30));
                await _gitStore.InsertOrUpdateAsync(log.GetGitStorePath(), JsonHelper.Serialize(logs));
            }
            catch { }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<List<SwanLog>> GetAsync(CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                return JsonHelper.Deserialize<List<SwanLog>>(await _gitStore.GetTextAsync(new SwanLog().GetGitStorePath(), cancellationToken: cancellationToken));
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
