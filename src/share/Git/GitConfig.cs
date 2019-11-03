namespace Laobian.Share.Git
{
    public class GitConfig
    {
        public string GitHubAccessToken { get; set; }

        public string GitHubRepositoryName { get; set; }

        public string GitHubRepositoryOwner { get; set; }

        public string GitHubRepositoryBranch { get; set; }

        public string GitCloneToDir { get; set; }

        public string GitCommitUser { get; set; }

        public string GitCommitEmail { get; set; }
    }
}
