using System.Drawing;
using Laobian.Share.BlogEngine;

namespace Laobian.Blog.Helpers
{
    public class HtmlHeaderHelper
    {
        public static string BuildMetaDescription(dynamic desc)
        {
            string str = desc?.ToString();
            if (string.IsNullOrEmpty(str))
            {
                str = string.Empty;
            }
            else
            {
                str += "... ";
            }

            str += BlogConstant.BlogDescription;

            return str.Length >= 150 ? str.Substring(0, 150) : str;
        }

        public static string BuildTitle(dynamic title)
        {
            string t = title?.ToString();
            return string.IsNullOrEmpty(t) ? BlogConstant.BlogName : $"{title} - {BlogConstant.BlogName}";
        }

        public static string BuildMetaRobots(dynamic robots)
        {
            string r = robots?.ToString();
            if (!string.IsNullOrEmpty(r))
            {
                return r;
            }

            if (!BlogState.IsProdEnvironment)
            {
                return "noindex, nofollow";
            }

            return "index, follow, archive";
        }
    }
}
