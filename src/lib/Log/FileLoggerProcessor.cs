using Laobian.Lib.Helper;
using Laobian.Lib.Option;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;

namespace Laobian.Lib.Log
{
    public class FileLoggerProcessor : IFileLoggerProcessor
    {
        private readonly object _lock;
        private readonly Thread _outputThread;
        private readonly LaobianOption _option;
        private readonly ConcurrentQueue<LaobianLog> _messageQueue;

        private bool _isAddingCompleted;

        public FileLoggerProcessor(IOptions<LaobianOption> option)
        {
            _lock = new object();
            _option = option.Value;
            _messageQueue = new ConcurrentQueue<LaobianLog>();
            _outputThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "File logger queue processing thread"
            };
            _outputThread.Start();
        }

        public void Dispose()
        {
            CompleteAdding();
            try
            {
                _outputThread.Join(5000);
            }
            catch { }
        }

        public void Ingest(LaobianLog log)
        {
            if(!Enqueue(log))
            {
                Write(log);
            }
        }

        private void ProcessLogQueue()
        {
            while (!_isAddingCompleted)
            {
                if(_messageQueue.TryDequeue(out var item))
                {
                    Write(item);
                }
            }

            while(_messageQueue.TryDequeue(out var item))
            {
                Write(item);
            }
        }

        private bool Enqueue(LaobianLog log)
        {
            if (!_isAddingCompleted)
            {
                _messageQueue.Enqueue(log);
                return true;
            }

            return false;
        }

        private void CompleteAdding()
        {
            lock(_messageQueue)
            {
                _isAddingCompleted = true;
            }
        }

        private void Write(LaobianLog log)
        {
            lock(_lock)
            {
                try
                {
                    var file = GetLogFileName(log.Timestamp);
                    var logs = new List<LaobianLog>();
                    if (File.Exists(file))
                    {
                        logs.AddRange(JsonHelper.Deserialize<IEnumerable<LaobianLog>>(File.ReadAllText(file, Encoding.UTF8)));
                    }

                    logs.Add(log);
                    File.WriteAllText(file, JsonHelper.Serialize(logs));
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Write file log failed. Log={JsonHelper.Serialize(log)}. Error={ex}");
                }
            }
        }

        private string GetLogFileName(DateTime timestamp)
        {
            var dir = Path.Combine(_option.AssetLocation, Constants.FolderAsset, Constants.FolderTemp, Constants.FolderTempLog);
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, $"{timestamp.Year:D4}-{timestamp.Month:D2}-{timestamp.Day:D2}.log");
        }
    }
}
