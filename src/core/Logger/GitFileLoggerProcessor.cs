using Swan.Core.Model;
using Swan.Core.Service;
using System.Collections.Concurrent;

namespace Swan.Core.Logger
{
    internal class GitFileLoggerProcessor : IGitFileLoggerProcessor
    {
        private readonly Thread _thread;
        private readonly ISwanLogService _swanLogService;
        private readonly BlockingCollection<SwanLog> _logs;

        public GitFileLoggerProcessor(ISwanLogService swanLogService)
        {
            _swanLogService = swanLogService;
            _logs = [];

            _thread = new Thread(Process)
            {
                IsBackground = true,
                Name = "GitFile logger processor"
            };
            _thread.Start();
        }

        public void Dispose()
        {
            _logs.CompleteAdding();

            try
            {
                _thread.Join();
            }
            catch { }
        }

        public void Ingest(SwanLog log)
        {
            if (_logs.IsAddingCompleted)
            {
                _swanLogService.AddAsync(log).Wait();
                return;
            }

            _logs.Add(log);
        }

        private void Process()
        {
            foreach (var log in _logs.GetConsumingEnumerable())
            {
                _swanLogService.AddAsync(log).Wait();
            }
        }
    }
}
