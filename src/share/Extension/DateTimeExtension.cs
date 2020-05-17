using System;

namespace Laobian.Share.Extension
{
    /// <summary>
    ///     Extensions for <see cref="DateTime" />
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        ///     Display time as date and time combination
        /// </summary>
        /// <param name="time">The given time</param>
        /// <returns>Combination of date and time part</returns>
        public static string ToDateAndTime(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        ///     Display time as date part
        /// </summary>
        /// <param name="time">The given time</param>
        /// <returns>Date part format</returns>
        public static string ToDate(this DateTime time)
        {
            return time.ToString("yyyy年MM月dd日");
        }
    }
}