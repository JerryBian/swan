using System;

namespace Laobian.Share.Extension
{
    public static class TimeSpanExtension
    {
        public static string ToHuman(this TimeSpan interval)
        {
            var result = string.Empty;
            if (interval.Days > 0)
            {
                result += $"{interval.Days}天";
            }

            if (interval.Hours > 0)
            {
                result += $"{interval.Hours}小时";
            }

            if (interval.Minutes > 0)
            {
                result += $"{interval.Hours}分钟";
            }

            if (string.IsNullOrEmpty(result))
            {
                result = interval.ToString();
            }

            return result;
        }
    }
}