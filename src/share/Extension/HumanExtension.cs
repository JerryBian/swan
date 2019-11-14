using System;
using Humanizer;

namespace Laobian.Share.Extension
{
    public static class HumanExtension
    {
        public static string Human(this int number)
        {
            return number.ToMetric(decimals: 1);
        }

        public static string Human(this TimeSpan interval)
        {
            return interval.Humanize(maxUnit: Humanizer.Localisation.TimeUnit.Year);
        }

        public static string Human(this DateTime time)
        {
            return time.Humanize(false);
        }
    }
}
