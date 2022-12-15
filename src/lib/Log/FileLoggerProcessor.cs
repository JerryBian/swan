using Swan.Lib.Helper;
using Swan.Lib.Service;
using System.Collections.Concurrent;

namespace Swan.Lib.Log
{
    public class FileLoggerProcessor : IFileLoggerProcessor
    {
        private readonly object _lock;
        private readonly Thread _outputThread;
        private readonly ILogService _logService;
        private readonly ConcurrentQueue<SwanLog> _messageQueue;

        private bool _isAddingCompleted;

        public FileLoggerProcessor(ILogService logService)
        {
            _lock = new object();
            _logService = logService;
            _messageQueue = new ConcurrentQueue<SwanLog>();
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
                _ = _outputThread.Join(5000);
            }
            catch { }
        }

        public void Ingest(SwanLog log)
        {
            if (!Enqueue(log))
            {
                Write(log);
            }
        }

        private void ProcessLogQueue()
        {
            while (!_isAddingCompleted)
            {
                if (_messageQueue.TryDequeue(out SwanLog item))
                {
                    Write(item);
                }
            }

            while (_messageQueue.TryDequeue(out SwanLog item))
            {
                Write(item);
            }
        }

        private bool Enqueue(SwanLog log)
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
            lock (_messageQueue)
            {
                _isAddingCompleted = true;
            }
        }

        private void Write(SwanLog log)
        {
            lock (_lock)
            {
                try
                {
                    _logService.AddLog(log);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Write file log failed. Log={JsonHelper.Serialize(log)}. Error={ex}");
                }
            }
        }
    }
}
