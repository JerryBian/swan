using System.Collections.Concurrent;

namespace Laobian.Share.Logger
{
    public class LaobianLogQueue : ILaobianLogQueue
    {
        private readonly ConcurrentQueue<LaobianLog> _logs;

        public LaobianLogQueue()
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