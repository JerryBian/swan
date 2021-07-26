using System.Collections.Concurrent;
using Laobian.Share.Logger;

namespace Laobian.Api.Logger
{
    public class GitFileLogQueue : IGitFileLogQueue
    {
        private readonly ConcurrentQueue<LaobianLog> _logs;

        public GitFileLogQueue()
        {
            _logs = new ConcurrentQueue<LaobianLog>();
        }

        public void Add(LaobianLog log)
        {
            _logs.Enqueue(log);
        }

        public bool TryDequeue(out LaobianLog log)
        {
            return _logs.TryDequeue(out log);
        }
    }
}