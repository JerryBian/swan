using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laobian.Api
{
    public class SystemLocker
    {
        private readonly ManualResetEventSlim _eventSlim;

        public SystemLocker()
        {
            _eventSlim = new ManualResetEventSlim(true);
        }

        public void Acquire()
        {
            _eventSlim.Wait();
        }

        public void Pause()
        {
            _eventSlim.Reset();
        }

        public void Resume()
        {
            _eventSlim.Set();
        }
    }
}
