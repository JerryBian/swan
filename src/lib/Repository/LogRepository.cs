using Laobian.Lib.Helper;
using Laobian.Lib.Log;
using Laobian.Lib.Provider;
using System.Text;

namespace Laobian.Lib.Repository
{
    public class LogRepository : ILogRepository
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IAssetFileProvider _assetFileProvider;

        public LogRepository(IAssetFileProvider assetFileProvider)
        {
            _assetFileProvider = assetFileProvider;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public void AddLog(LaobianLog log)
        {
            _semaphoreSlim.Wait();
            try
            {
                var dir = _assetFileProvider.GetLogBaseDir();
                var timestamp = log.Timestamp;
                var file = Path.Combine(dir, $"{timestamp.Year:D4}-{timestamp.Month:D2}-{timestamp.Day:D2}{_assetFileProvider.LogExtension}");
                var logs = new List<LaobianLog>();
                if (File.Exists(file))
                {
                    logs.AddRange(GetLogs(file));
                }

                logs.Add(log);
                File.WriteAllText(file, JsonHelper.Serialize(logs));
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public List<LaobianLog> ReadAllAsync(LogLevel minLogLevel)
        {
            _semaphoreSlim.Wait();
            try
            {
                var logs = new List<LaobianLog>();
                foreach (var file in Directory.EnumerateFiles(_assetFileProvider.GetLogBaseDir(), $"*{_assetFileProvider.LogExtension}", SearchOption.AllDirectories))
                {
                    var fileLogs = GetLogs(file);
                    logs.AddRange(fileLogs.Where(x => x.Level >= minLogLevel));
                }

                return logs;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private List<LaobianLog> GetLogs(string file)
        {
            var content = File.ReadAllText(file, Encoding.UTF8);
            var logs = JsonHelper.Deserialize<List<LaobianLog>>(content);
            return logs;
        }
    }
}
