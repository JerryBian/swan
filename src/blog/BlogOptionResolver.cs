using Laobian.Share.Option;
using Microsoft.Extensions.Configuration;

namespace Laobian.Blog
{
    public class BlogOptionResolver : CommonOptionResolver
    {
        public void Resolve(BlogOption option, IConfiguration configuration)
        {
            base.Resolve(option, configuration);
            option.GitHubHookSecret = configuration.GetValue<string>("GITHUB_HOOK_SECRET");
            option.PostsPerPage = configuration.GetValue<int>("POSTS_PER_PAGE");
        }
    }
}