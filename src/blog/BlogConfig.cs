using Laobian.Share.Config;

namespace Laobian.Blog
{
    public class BlogConfig : CommonConfig
    {
        public int PostsPerPage { get; set; }

        public string AdminChineseName { get; set; }

        public string AdminEnglishName { get; set; }

        public string GitHubHookSecret { get; set; }
    }
}