using Laobian.Lib.Helper;
using Laobian.Lib.Option;
using Laobian.Lib.Provider;
using Laobian.Lib.Service;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;

namespace Laobian.Lib.Log
{
    public class FileLoggerProcessor : IFileLoggerProcessor
    {
        private readonly object _lock;
        private readonly Thread _outputThread;
        private readonly ILogService _logService;
        private readonly ConcurrentQueue<LaobianLog> _messageQueue;

        private bool _isAddingCompleted;

        public FileLoggerProcessor(ILogService logService)
        {
            _lock = new object();
            _logService = logService;
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
                    _logService.AddLog(log);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Write file log failed. Log={JsonHelper.Serialize(log)}. Error={ex}");
                }
            }
        }
    }
}
