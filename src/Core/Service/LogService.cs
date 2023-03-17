using Swan.Core.Model.Object;
using Swan.Core.Store;

namespace Swan.Core.Service
{
    public class LogService : ILogService
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IFileObjectStore<LogObject> _store;

        private bool _started;

        public LogService(IFileObjectStore<LogObject> store)
        {
            _store = store;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public void Start()
        {
            _started = true;
        }

        public bool HasStarted()
        {
            return _started;
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
                await _store.DeleteAsync(x => x.Timestamp < DateTime.Now.AddDays(-30));
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }
    }
}
