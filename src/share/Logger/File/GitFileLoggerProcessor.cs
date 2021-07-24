using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Laobian.Share.Helper;

namespace Laobian.Share.Logger.File
{
    public class GitFileLoggerProcessor : ILaobianLoggerProcessor, IDisposable
    {
        private readonly ConcurrentQueue<LaobianLog> _messageQueue;
        private readonly GitFileLoggerOptions _options;
        private readonly Thread _underlingThread;
        private bool _stop;

        public GitFileLoggerProcessor(GitFileLoggerOptions options)
        {
            _options = options;
            _messageQueue = new ConcurrentQueue<LaobianLog>();

            _underlingThread = new Thread(Process)
            {
                IsBackground = true,
                Name = "Git file logs processing thread"
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
                ProcessLogs(log);
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
                    if (Directory.Exists(_options.LoggerDir))
                    {
                        var logs = new List<LaobianLog>();
                        while (_messageQueue.TryDequeue(out var log))
                        {
                            logs.Add(log);
                        }

                        if (logs.Any())
                        {
                            ProcessLogs(logs.ToArray());
                        }

                        if (_stop)
                        {
                            return;
                        }
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
            catch (Exception)
            {
                _stop = true;
            }
        }

        private void ProcessLogs(params LaobianLog[] logs)
        {
            if (logs == null || !logs.Any())
            {
                return;
            }

            if (string.IsNullOrEmpty(_options.LoggerDir))
            {
                foreach (var log in logs)
                {
                    Console.WriteLine(log);
                }

                return;
            }

            foreach (var log in logs)
            {
                try
                {
                    var dir = Path.Combine(_options.LoggerDir, log.TimeStamp.Year.ToString(),
                        log.TimeStamp.Month.ToString("D2"));
                    Directory.CreateDirectory(dir);
                    System.IO.File.AppendAllLines(Path.Combine(dir, $"{log.TimeStamp:yyyy-MM-dd}.txt"),
                        new[] {JsonHelper.Serialize(log)});
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}