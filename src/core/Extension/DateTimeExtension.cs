namespace Swan.Core.Extension
{
    public static class DateTimeExtension
    {
        public static string ToCnDate(this DateTime time, bool noYear = false)
        {
            return noYear ? time.ToString("MM月dd日") : time.ToString("yyyy年MM月dd日");
        }

        public static string ToDate(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }
    }
}
