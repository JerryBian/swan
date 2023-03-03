using Swan.Core.Helper;
using Swan.Core.Model.Object;
using Swan.Core.Service;
using System.Collections.Concurrent;

namespace Swan.Core.Log
{
    public class FileLoggerProcessor : IFileLoggerProcessor
    {
        private readonly SemaphoreSlim _lock;
        private readonly Task _task;
        private readonly ILogService _logService;
        private readonly ConcurrentQueue<LogObject> _messageQueue;

        private bool _isAddingCompleted;

        public FileLoggerProcessor(ILogService logService)
        {
            _lock = new SemaphoreSlim(1, 1);
            _logService = logService;
            _messageQueue = new ConcurrentQueue<LogObject>();
            _task = Task.Run(ProcessLogQueue);
        }

        public void Dispose()
        {
            CompleteAdding();
            _task.Wait();
        }

        public void Ingest(LogObject log)
        {
            if (!Enqueue(log))
            {
                WriteAsync(log).Wait();
            }
        }

        private async Task ProcessLogQueue()
        {
            while (!_isAddingCompleted)
            {
                bool hasItems = false;
                while (_messageQueue.TryDequeue(out LogObject item))
                {
                    hasItems = true;
                    await WriteAsync(item);
                }

                if (!hasItems && !_isAddingCompleted)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }

            while (_messageQueue.TryDequeue(out LogObject item))
            {
                await WriteAsync(item);
            }
        }

        private bool Enqueue(LogObject log)
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

        private async Task WriteAsync(LogObject log)
        {
            await _lock.WaitAsync();

            try
            {
                await _logService.AddLogAsync(log);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Write file log failed. Log={JsonHelper.Serialize(log)}. Error={ex}");
            }
            finally
            {
                _ = _lock.Release();
            }
        }
    }
}
