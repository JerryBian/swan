using Laobian.Share.Option;

namespace Laobian.Api
{
    public class LaobianApiOption : LaobianSharedOption
    {
        [OptionEnvName("COMMAND_LINE_APP")] public string CommandLineApp { get; set; }

        [OptionEnvName("COMMAND_LINE_BEGIN_ARG")]
        public string CommandLineBeginArg { get; set; }

        [OptionEnvName("GITHUB_DB_REPO_API_TOKEN")]
        public string GitHubDbRepoApiToken { get; set; }

        [OptionEnvName("GITHUB_DB_REPO_USER_NAME")]
        public string GitHubDbRepoUserName { get; set; }

        [OptionEnvName("GITHUB_DB_REPO_NAME")] public string GitHubDbRepoName { get; set; }

        [OptionEnvName("GITHUB_DB_REPO_BRANCH_NAME")]
        public string GitHubDbRepoBranchName { get; set; }
    }
}