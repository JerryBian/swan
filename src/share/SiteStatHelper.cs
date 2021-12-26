using System.Diagnostics;

namespace Laobian.Share
{
    public class SiteStatHelper
    {
        public static SiteStat Get()
        {
            using var process = Process.GetCurrentProcess();
            var stat = new SiteStat
            {
                AllocatedPhysicalMemory = process.WorkingSet64,
                AllocatedVirtualMemory = process.VirtualMemorySize64,
                MaximumAllocatedPhysicalMemory = process.PeakWorkingSet64,
                MaximumAllocatedVirtualMemory = process.PeakVirtualMemorySize64,
                StartTime = process.StartTime,
                Threads = process.Threads.Count,
                TotalProcessorTime = process.TotalProcessorTime
            };

            return stat;
        }
    }
}
