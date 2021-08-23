using Laobian.Share.Option;

namespace Laobian.Blog
{
    public class BlogOption : CommonOption
    {
        [OptionEnvName("POSTS_PER_PAGE")]
        public string PostsPerPage { get; set; }
    }
}