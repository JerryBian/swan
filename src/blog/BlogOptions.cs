using Laobian.Share.Option;

namespace Laobian.Blog;

public class BlogOptions : SharedOptions
{
    [OptionEnvName("POSTS_PER_PAGE")] public string PostsPerPage { get; set; }
}