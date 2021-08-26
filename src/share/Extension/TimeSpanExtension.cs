using System;

namespace Laobian.Share.Extension
{
    public static class TimeSpanExtension
    {
        public static string ToHuman(this TimeSpan interval)
        {
            return interval.ToString("dd天hh小时mm分钟");
        }
    }
}