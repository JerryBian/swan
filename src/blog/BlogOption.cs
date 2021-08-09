using Laobian.Share.Option;

namespace Laobian.Blog
{
    public class BlogOption : CommonOption
    {
        public int PostsPerPage { get; set; }

        public string GitHubHookSecret { get; set; }
    }
}