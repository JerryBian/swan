using System.Threading;

namespace Laobian.Api
{
    public class SystemLocker
    {
        public SystemLocker()
        {
            FileLockResetEvent = new ManualResetEventSlim(true);
        }

        public ManualResetEventSlim FileLockResetEvent { get; init; }
    }
}