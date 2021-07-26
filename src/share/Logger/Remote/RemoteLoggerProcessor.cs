using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Helper;

namespace Laobian.Share.Logger.Remote
{
    public class RemoteLoggerProcessor : ILaobianLoggerProcessor, IDisposable
    {
        private readonly ConcurrentQueue<LaobianLog> _messageQueue;
        private readonly RemoteLoggerOptions _options;
        private readonly IRemoteLoggerSink _sink;
        private readonly Thread _underlingThread;

        private bool _stop;

        public RemoteLoggerProcessor(RemoteLoggerOptions options, IRemoteLoggerSink sink)
        {
            _sink = sink;
            _options = options;
            _messageQueue = new ConcurrentQueue<LaobianLog>();

            _underlingThread = new Thread(Process)
            {
                IsBackground = true,
                Name = $"Remote logs processing thread for {options.LoggerName}"
            };
            _underlingThread.Start();
        }

        public void Dispose()
        {
            _stop = true;
            try
            {
                _underlingThread.Join(TimeSpan.FromSeconds(15));
            }
            catch (ThreadStateException)
            {
            }
        }

        public void Add(LaobianLog log)
        {
            if (!_stop)
            {
                try
                {
                    _messageQueue.Enqueue(log);
                    return;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            try
            {
                ProcessLogsAsync(log).Wait();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void Process()
        {
            try
            {
                while (true)
                {
                    var logs = new List<LaobianLog>();
                    while (_messageQueue.TryDequeue(out var log))
                    {
                        logs.Add(log);
                    }

                    if (logs.Any())
                    {
                        ProcessLogsAsync(logs.ToArray()).Wait();
                    }

                    if (_stop)
                    {
                        return;
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
            catch (Exception)
            {
                _stop = true;
            }
        }

        private async Task ProcessLogsAsync(params LaobianLog[] logs)
        {
            if (logs == null || !logs.Any())
            {
                return;
            }

            try
            {
                await _sink.SendAsync(_options.LoggerName, logs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sink failed.{Environment.NewLine}{ex}");
                foreach (var log in logs)
                {
                    Console.WriteLine(JsonHelper.Serialize(log));
                }
            }
        }
    }
}