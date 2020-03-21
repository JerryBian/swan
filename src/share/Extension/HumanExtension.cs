using System;
using Humanizer;
using Humanizer.Localisation;

namespace Laobian.Share.Extension
{
    public static class HumanExtension
    {
        public static string Human(this int number)
        {
            return number.ToMetric(decimals: 1);
        }

        public static string HumanByte(this long bytes)
        {
            return bytes.Bytes().ToString("#.##MB");
        }

        public static string Human(this TimeSpan interval, int precision = 2)
        {
            return interval.Humanize(maxUnit: TimeUnit.Year, precision: precision);
        }

        public static string Human(this DateTime time)
        {
            return time.Humanize(false);
        }
    }
}