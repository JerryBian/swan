using Laobian.Share.Option;

namespace Laobian.Blog
{
    public class BlogConfig : CommonOption
    {
        public int PostsPerPage { get; set; }

        public string AdminChineseName { get; set; }

        public string AdminEnglishName { get; set; }

        public string GitHubHookSecret { get; set; }
    }
}