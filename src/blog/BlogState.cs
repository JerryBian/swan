using System;
using System.Runtime.InteropServices;
using Humanizer;

namespace Laobian.Blog
{
    public class BlogState
    {
        private static DateTime _startAt = DateTime.UtcNow;

        public static void Init()
        {
            if (_startAt != default)
            {
                _startAt = DateTime.UtcNow;
            }
        }

        public static string NetCoreVersion => RuntimeInformation.FrameworkDescription;

        public static string RunningInterval => (DateTime.UtcNow - _startAt).Humanize();
    }
}
