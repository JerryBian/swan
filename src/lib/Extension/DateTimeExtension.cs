namespace Laobian.Lib.Extension
{
    public static class DateTimeExtension
    {
        public static string ToDate(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }

        public static string ToCnDate(this DateTime time)
        {
            return time.ToString("yyyy年MM月dd日");
        }

        public static string ToTime(this DateTime time)
        {
            return time.ToString("HH:mm:ss");
        }
    }
}
