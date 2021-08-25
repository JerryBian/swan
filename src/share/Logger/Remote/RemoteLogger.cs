using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Logger.Remote
{
    public class RemoteLogger : ILogger
    {
        private readonly ILaobianLogQueue _logQueue;

        public RemoteLogger(ILaobianLogQueue logQueue)
        {
            _logQueue = logQueue;
        }

        public IExternalScopeProvider ScopeProvider { get; set; }

        public ILaobianLoggerOptions Options { get; set; }

        public IDisposable BeginScope<TState>(TState state)
        {
            return ScopeProvider?.Push(state) ?? RemoteNullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= Options.MinLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            try
            {
                var log = GetScopeInfo();
                log.Message = formatter(state, exception);
                log.Exception = exception?.ToString();
                log.Level = logLevel;
                log.TimeStamp = DateTime.Now;
                _logQueue.Add(log);
                Console.WriteLine("a");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private LaobianLog GetScopeInfo()
        {
            var log = new LaobianLog();
            if (ScopeProvider == null)
            {
                return log;
            }

            ScopeProvider.ForEachScope((o, otherLog) =>
            {
                if (o is LaobianLog logObj)
                {
                    log.Clone(logObj);
                }
            }, log);

            return log;
        }
    }
}