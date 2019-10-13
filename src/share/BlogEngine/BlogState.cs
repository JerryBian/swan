using System;
using System.Runtime.InteropServices;
using Humanizer;

namespace Laobian.Share.BlogEngine
{
    public class BlogState
    {
        public static DateTime StartAtUtc { get; set; }

        public static string NetCoreVersion => RuntimeInformation.FrameworkDescription;

        public static string RunningInterval => (DateTime.UtcNow - StartAtUtc).Humanize();

        public static bool IsDevEnvironment { get; set; } = false;

        public static bool IsStageEnvironment { get; set; } = false;

        public static bool IsProdEnvironment { get; set; } = false;
    }
}
