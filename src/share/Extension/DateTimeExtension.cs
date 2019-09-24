using System;

namespace Laobian.Share.Extension
{
    /// <summary>
    /// Extensions for <see cref="DateTime"/>
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// Display UTC time as relative
        /// </summary>
        /// <param name="time">The given time</param>
        /// <returns>Friendly display string as relative time</returns>
        public static string ToRelativeTime(this DateTime time)
        {
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - time.Ticks);
            var delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * minute)
                return ts.Seconds == 1 ? "1 分钟前" : ts.Seconds + " 分钟前";

            if (delta < 2 * minute)
                return "1 分钟前";

            if (delta < 45 * minute)
                return ts.Minutes + " 分钟前";

            if (delta < 90 * minute)
                return "1 小时前";

            if (delta < 24 * hour)
                return ts.Hours + " 小时前";

            if (delta < 48 * hour)
                return "昨天";

            if (delta < 30 * day)
                return ts.Days + " 天前";

            if (delta < 12 * month)
            {
                var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "1 月前" : months + " 月前";
            }

            return time.ToDate();
        }

        /// <summary>
        /// Convert UTC time to China time
        /// </summary>
        /// <param name="utcTime">The given UTC time</param>
        /// <returns>China time</returns>
        /// <remarks>
        /// Given time must be UTC, otherwise the result will be incorrect
        /// </remarks>
        public static DateTime ToChinaTime(this DateTime utcTime)
        {
            return utcTime.AddHours(8);
        }

        /// <summary>
        /// Display time as ISO8601 format
        /// </summary>
        /// <param name="time">The given time</param>
        /// <returns>ISO8601 format time</returns>
        public static string ToIso8601(this DateTime time)
        {
            return time.ToString("O");
        }

        /// <summary>
        /// Display time as date and time combination
        /// </summary>
        /// <param name="time">The given time</param>
        /// <returns>Combination of date and time part</returns>
        public static string ToDateAndTime(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Display time as date and time combination in lite version
        /// </summary>
        /// <param name="time">The given time</param>
        /// <returns>Combination of date and time part in lite version</returns>
        public static string ToDateAndTimeLite(this DateTime time)
        {
            return time.ToString("yyyyMMddHHmmssfff");
        }

        /// <summary>
        /// Display time as date part
        /// </summary>
        /// <param name="time">The given time</param>
        /// <returns>Date part format</returns>
        public static string ToDate(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Display time as lite date part
        /// </summary>
        /// <param name="time">The given time</param>
        /// <returns>Lite date string</returns>
        public static string ToDateLite(this DateTime time)
        {
            return time.ToString("yyyyMMdd");
        }

        /// <summary>
        /// Display time as lite month part
        /// </summary>
        /// <param name="time">The given time</param>
        /// <returns>Lite year and month string</returns>
        public static string ToMonthLite(this DateTime time)
        {
            return time.ToString("yyyyMM");
        }
    }
}
