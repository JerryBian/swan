using Swan.Core.Model;
using Swan.Core.Model.Object;
using Swan.Core.Store;

namespace Swan.Core.Service
{
    public class LogService : ILogService
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IFileObjectStore<LogObject> _store;

        public LogService(IFileObjectStore<LogObject> store)
        {
            _store = store;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task<List<SwanLog>> GetAllLogsAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var logs = await _store.GetAllAsync();
                var result = new List<SwanLog>();
                foreach (var log in logs.OrderByDescending(x => x.Timestamp))
                {
                    result.Add(new SwanLog(log));
                }

                return result;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task AddLogAsync(SwanLog log)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                await _store.AddAsync(log.Raw);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task CleanupAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var logs = await _store.GetAllAsync();
                foreach (var log in logs.Where(x => x.Timestamp < DateTime.Now.AddDays(-30)))
                {
                    await _store.DeleteAsync(log.Id);
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
