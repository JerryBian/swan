using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Laobian.Share.Log;

namespace Laobian.Share
{
    public static class MemoryStore
    {
        public static ConcurrentQueue<LogEntry> LogQueue = new ConcurrentQueue<LogEntry>();
    }
}
