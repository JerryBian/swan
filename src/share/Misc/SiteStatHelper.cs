using System.Diagnostics;
using ByteSizeLib;
using Laobian.Share.Extension;

namespace Laobian.Share.Misc;

public class SiteStatHelper
{
    public static SiteStat Get()
    {
        using var process = Process.GetCurrentProcess();
        var stat = new SiteStat
        {
            AllocatedPhysicalMemory = ByteSize.FromBytes(process.WorkingSet64).ToString(),
            MaximumAllocatedPhysicalMemory = ByteSize.FromBytes(process.PeakWorkingSet64).ToString(),
            StartTime = process.StartTime,
            Threads = process.Threads.Count,
            TotalProcessorTime = process.TotalProcessorTime.ToHuman()
        };

        return stat;
    }
}