using Laobian.Share.Option;

namespace Laobian.Blog
{
    public class LaobianBlogOption : LaobianSharedOption
    {
        [OptionEnvName("POSTS_PER_PAGE")] public string PostsPerPage { get; set; }
    }
}