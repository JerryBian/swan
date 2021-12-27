using System.Diagnostics;
using ByteSizeLib;
using Laobian.Share.Extension;

namespace Laobian.Share
{
    public class SiteStatHelper
    {
        public static SiteStat Get()
        {
            using var process = Process.GetCurrentProcess();
            var stat = new SiteStat
            {
                AllocatedPhysicalMemory = ByteSize.FromBytes(process.WorkingSet64).ToString("#.# MB"),
                MaximumAllocatedPhysicalMemory = ByteSize.FromBytes(process.PeakWorkingSet64).ToString("#.# MB"),
                StartTime = process.StartTime,
                Threads = process.Threads.Count,
                TotalProcessorTime = process.TotalProcessorTime.ToHuman()
            };

            return stat;
        }
    }
}
