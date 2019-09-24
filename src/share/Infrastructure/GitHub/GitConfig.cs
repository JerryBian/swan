namespace Laobian.Share.Infrastructure.GitHub
{
    public class GitConfig
    {
        public string GitHubAccessToken { get; set; }

        public string GitHubRepositoryName { get; set; }

        public string GitHubRepositoryOwner { get; set; }

        public string GitHubRepositoryBranch { get; set; }

        public string GitCloneToDir { get; set; }
    }
}
