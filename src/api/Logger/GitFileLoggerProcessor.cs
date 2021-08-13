﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Laobian.Share.Logger;
using Laobian.Share.Util;

namespace Laobian.Api.Logger
{
    public class GitFileLoggerProcessor : ILaobianLoggerProcessor, IDisposable
    {
        private readonly ILaobianLogQueue _messageQueue;
        private readonly GitFileLoggerOptions _options;
        private readonly SystemLocker _systemLocker;
        private readonly Thread _underlingThread;

        private bool _stop;

        public GitFileLoggerProcessor(GitFileLoggerOptions options, ILaobianLogQueue messageQueue,
            SystemLocker systemLocker)
        {
            _options = options;
            _systemLocker = systemLocker;
            Directory.CreateDirectory(options.BaseDir);
            _messageQueue = messageQueue;

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
                    _messageQueue.Add(log);
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

            foreach (var log in logs)
            {
                try
                {
                    log.LoggerName = string.IsNullOrEmpty(log.LoggerName) ? _options.LoggerName : log.LoggerName;
                    var dir = Path.Combine(_options.BaseDir, log.LoggerName, log.TimeStamp.Year.ToString(),
                        log.TimeStamp.Month.ToString("D2"));

                    try
                    {
                        _systemLocker.FileLockResetEvent.Wait();
                        _systemLocker.FileLockResetEvent.Reset();
                        Directory.CreateDirectory(dir);
                        File.AppendAllLines(Path.Combine(dir, $"{log.TimeStamp:yyyy-MM-dd}.log"),
                            new[] {JsonUtil.Serialize(log)});
                    }
                    finally
                    {
                        _systemLocker.FileLockResetEvent.Set();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Process Logs failed. ==> {JsonUtil.Serialize(log)}{Environment.NewLine}{ex}");
                }
            }
        }
    }
}