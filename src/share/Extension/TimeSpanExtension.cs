using System;

namespace Laobian.Share.Extension
{
    public static class TimeSpanExtension
    {
        public static string ToDisplayString(this TimeSpan interval)
        {
            return interval.ToString(@"dd\.hh\:mm\:ss");
        }
    }
}