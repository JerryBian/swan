using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Log;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Hubs
{
    public class LogHub : Hub
    {
        public async IAsyncEnumerable<IEnumerable<LogEntry>> Get(
            string type,
            [EnumeratorCancellation]
            CancellationToken cancellationToken)
        {
            var lastTimestamp = default(DateTime);
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var logs = GetLogs(type);
                var timestamp = lastTimestamp;
                yield return logs.Where(l => l.When > timestamp);

                lastTimestamp = logs.FirstOrDefault()?.When ?? default;
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        private List<LogEntry> GetLogs(string type)
        {
            var logs = new List<LogEntry>();
            var entries = Global.InMemoryLogQueue.ToList();
            if (type == "all_logs")
            {
                logs.AddRange(entries);
            }
            else if (type == "warn_logs")
            {
                logs.AddRange(entries.Where(e => e.Level == LogLevel.Warning));
            }
            else if (type == "error_logs")
            {
                logs.AddRange(entries.Where(e => e.Level == LogLevel.Error));
            }
            else if (type == "info_logs")
            {
                logs.AddRange(entries.Where(e => e.Level == LogLevel.Information));
            }

            return logs;
        }
    }
}
