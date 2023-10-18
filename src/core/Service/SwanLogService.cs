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
                List<SwanLog> logs = JsonHelper.Deserialize<List<SwanLog>>(await _gitStore.GetTextAsync(SwanLog.GitStorePath, true));
                logs ??= new List<SwanLog>();
                logs.Add(log);

                logs.RemoveAll(x => DateTime.Now.Date - x.CreatedAt.Date > TimeSpan.FromDays(30));
                await _gitStore.InsertOrUpdateAsync(SwanLog.GitStorePath, JsonHelper.Serialize(logs), true);
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
                return JsonHelper.Deserialize<List<SwanLog>>(await _gitStore.GetTextAsync(SwanLog.GitStorePath, true, cancellationToken: cancellationToken));
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
