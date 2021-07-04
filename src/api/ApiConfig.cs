﻿namespace Laobian.Api
{
    using Share;

    public class ApiConfig : CommonConfig
    {
        public SourceMode Source { get; set; }

        public string DbLocation { get; set; }

        public string BlogPostLocation { get; set; }

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