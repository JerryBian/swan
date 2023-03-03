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

        public async Task<List<LogObject>> GetAllLogsAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                IEnumerable<LogObject> logs = await _store.GetAllAsync();
                List<LogObject> result = new();
                foreach (LogObject log in logs.OrderByDescending(x => x.Timestamp))
                {
                    result.Add(log);
                }

                return result;
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task AddLogAsync(LogObject log)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                _ = await _store.AddAsync(log);
                IEnumerable<LogObject> logs = await _store.GetAllAsync();
                foreach (LogObject item in logs.Where(x => x.Timestamp < DateTime.Now.AddDays(-15)))
                {
                    await _store.DeleteAsync(item.Id);
                }
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }
    }
}
