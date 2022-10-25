using Laobian.Lib.Helper;
using Laobian.Lib.Log;
using Laobian.Lib.Option;
using Microsoft.Extensions.Options;
using System.Text;

namespace Laobian.Lib.Repository
{
    public class LogRepository : ILogRepository
    {
        private const string LogExt = ".log";
        private readonly LaobianOption _options;
        private readonly SemaphoreSlim _semaphoreSlim;

        public LogRepository(IOptions<LaobianOption> options)
        {
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _options = options.Value;
        }

        public void AddLog(LaobianLog log)
        {
            _semaphoreSlim.Wait();
            try
            {
                string dir = GetLogBaseDir();
                DateTime timestamp = log.Timestamp;
                string file = Path.Combine(dir, $"{timestamp.Year:D4}-{timestamp.Month:D2}-{timestamp.Day:D2}{LogExt}");
                List<LaobianLog> logs = new();
                if (File.Exists(file))
                {
                    logs.AddRange(GetLogs(file));
                }

                logs.Add(log);
                File.WriteAllText(file, JsonHelper.Serialize(logs));
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public List<LaobianLog> ReadAll(LogLevel minLogLevel)
        {
            _semaphoreSlim.Wait();
            try
            {
                List<LaobianLog> logs = new();
                foreach (string file in Directory.EnumerateFiles(GetLogBaseDir(), $"*{LogExt}", SearchOption.AllDirectories))
                {
                    List<LaobianLog> fileLogs = GetLogs(file);
                    logs.AddRange(fileLogs.Where(x => x.Level >= minLogLevel));
                }

                return logs;
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public void Cleanup()
        {
            _semaphoreSlim.Wait();
            try
            {
                foreach (string file in Directory.EnumerateFiles(GetLogBaseDir(), $"*{LogExt}", SearchOption.AllDirectories))
                {
                    DateTime lastModifiedAt = new FileInfo(file).LastWriteTime;
                    if (DateTime.Now - lastModifiedAt > TimeSpan.FromDays(7))
                    {
                        File.Delete(file);
                    }
                }
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        private List<LaobianLog> GetLogs(string file)
        {
            string content = File.ReadAllText(file, Encoding.UTF8);
            List<LaobianLog> logs = JsonHelper.Deserialize<List<LaobianLog>>(content);
            return logs;
        }

        public string GetLogBaseDir()
        {
            string dir = Path.Combine(_options.AssetLocation, Constants.FolderAsset, Constants.FolderTemp, Constants.FolderTempLog);
            _ = Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
