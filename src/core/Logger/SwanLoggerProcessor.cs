using Swan.Core.Model;
using System.Collections.Concurrent;

namespace Swan.Core.Logger
{
    internal class SwanLoggerProcessor : ISwanLoggerProcessor
    {
        private readonly Thread _thread;
        private readonly BlockingCollection<SwanLog> _logs;

        public SwanLoggerProcessor()
        {
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
                //_swanLogService.AddAsync(log).Wait();
                return;
            }

            _logs.Add(log);
        }

        private void Process()
        {
            foreach (var log in _logs.GetConsumingEnumerable())
            {
                //_swanLogService.AddAsync(log).Wait();
            }
        }
    }
}
