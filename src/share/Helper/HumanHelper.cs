using System;

namespace Laobian.Share.Helper
{
    public static class HumanHelper
    {
        public static string DisplayInt(int number)
        {
            if (number < 1000)
            {
                return number.ToString();
            }

            if (number < 1_000_000)
            {
                var i = (double) number / 1000;
                var s = i.ToString("#.#");
                return $"{s}k";
            }

            var j = (double) number / 1_000_000;
            var t = j.ToString("#.#");
            return $"{t}m";
        }

        public static string DisplayBytes(long bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes}B";
            }

            if (bytes < 1024 * 1024)
            {
                return $"{(double) bytes / 1024:#.#}k";
            }

            if (bytes < 1024 * 1024 * 1024)
            {
                return $"{(double) bytes / (1024 * 1024):#.#}M";
            }

            if (bytes < (long) 1024 * 1024 * 1024 * 1024)
            {
                return $"{(double) bytes / ((long) 1024 * 1024 * 1024):#.#}G";
            }

            return $"{(double) bytes / ((long) 1024 * 1024 * 1024 * 1024):#.#}T";
        }

        public static string DisplayTimeSpan(TimeSpan interval)
        {
            if (interval.TotalSeconds < 60)
            {
                return $"{interval:ss}秒";
            }

            if (interval.Days <= 0)
            {
                return $"{interval:hh}小时{interval:mm}分钟";
            }

            if (interval.Days > 365)
            {
                var year = interval.Days / 365;
                var days = interval.Days % 365;
                return $"{year}年{days}天{interval:hh}小时";
            }

            return $"{interval:dd}天{interval:hh}小时";
        }
    }
}