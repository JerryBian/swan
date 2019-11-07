using Laobian.Share.Config;

namespace Laobian.Blog.Helpers
{
    public class HtmlHeaderHelper
    {
        public static string BuildMetaDescription(AppConfig config, dynamic desc)
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

            str += config.Blog.Description;

            return str.Length >= 150 ? str.Substring(0, 150) : str;
        }

        public static string BuildTitle(AppConfig config, dynamic title)
        {
            string t = title?.ToString();
            return string.IsNullOrEmpty(t) ? config.Blog.Name : $"{title} - {config.Blog.Name}";
        }

        public static string BuildMetaRobots(dynamic robots)
        {
            string r = robots?.ToString();
            if (!string.IsNullOrEmpty(r))
            {
                return r;
            }

            //if (!BlogState.IsProdEnvironment)
            //{
            //    return "noindex, nofollow";
            //}

            return "index, follow, archive";
        }
    }
}
