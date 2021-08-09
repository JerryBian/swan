using Laobian.Share.Option;
using Microsoft.Extensions.Configuration;

namespace Laobian.Api
{
    public class ApiOptionResolver : CommonOptionResolver
    {
        public void Resolve(ApiOption option, IConfiguration configuration)
        {
            base.Resolve(option, configuration);
            option.Source = configuration.GetValue<SourceMode>("SOURCE");
            option.CommandLineApp = configuration.GetValue<string>("COMMAND_LINE_APP");
            option.CommandLineBeginArg = configuration.GetValue<string>("COMMAND_LINE_BEGIN_ARG");
            option.GitHubBlogPostRepoApiToken = configuration.GetValue<string>("GITHUB_BLOG_POST_REPO_API_TOKEN");
            option.GitHubBlogPostRepoBranchName = configuration.GetValue<string>("GITHUB_BLOG_POST_REPO_BRANCH_NAME");
            option.GitHubBlogPostRepoName = configuration.GetValue<string>("GITHUB_BLOG_POST_REPO_NAME");
            option.GitHubBlogPostRepoUserName = configuration.GetValue<string>("GITHUB_BLOG_POST_REPO_USER_NAME");
            option.GitHubDbRepoApiToken = configuration.GetValue<string>("GITHUB_DB_REPO_API_TOKEN");
            option.GitHubDbRepoBranchName = configuration.GetValue<string>("GITHUB_DB_REPO_BRANCH_NAME");
            option.GitHubDbRepoName = configuration.GetValue<string>("GITHUB_DB_REPO_NAME");
            option.GitHubDbRepoUserName = configuration.GetValue<string>("GITHUB_DB_REPO_USER_NAME");
        }
    }
}