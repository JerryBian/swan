namespace Swan.Core.Extension
{
    public static class DateTimeExtension
    {
        public static string ToCnDate(this DateTime time)
        {
            return time.ToString("yyyy年mm月dd日");
        }
    }
}
