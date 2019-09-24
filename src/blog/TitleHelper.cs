using Laobian.Share.BlogEngine;

namespace Laobian.Blog
{
    public class TitleHelper
    {
        public static string GetTitle(dynamic title)
        {
            var t = title?.ToString();
            return string.IsNullOrEmpty(t) ? BlogConstant.BlogName : $"{title} - {BlogConstant.BlogName}";
        }
    }
}
