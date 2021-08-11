using Laobian.Share.Option;

namespace Laobian.Api
{
    public class ApiOption : CommonOption
    {
        public SourceMode Source { get; set; }

        public string CommandLineApp { get; set; }

        public string CommandLineBeginArg { get; set; }

        public string GitHubDbRepoApiToken { get; set; }

        public string GitHubDbRepoUserName { get; set; }

        public string GitHubDbRepoName { get; set; }

        public string GitHubDbRepoBranchName { get; set; }

        public string GitHubBlogPostRepoApiToken { get; set; }

        public string GitHubBlogPostRepoUserName { get; set; }

        public string GitHubBlogPostRepoName { get; set; }

        public string GitHubBlogPostRepoBranchName { get; set; }
    }
}