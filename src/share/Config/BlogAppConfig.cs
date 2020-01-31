namespace Laobian.Share.Config
{
    public class BlogAppConfig : IConfig
    {
        [ConfigMeta(Name = "ASSET_GITHUB_REPO_API_TOKEN", Required = true)]
        public string AssetGitHubRepoApiToken { get; set; }

        [ConfigMeta(Name = "ASSET_GITHUB_REPO_BRANCH", Required = true)]
        public string AssetGitHubRepoBranch { get; set; }

        [ConfigMeta(Name = "ASSET_REPO_LOCAL_DIR", Required = true)]
        public string AssetRepoLocalDir { get; set; }

        [ConfigMeta(Name = "ASSET_GITHUB_REPO_OWNER", Required = true)]
        public string AssetGitHubRepoOwner { get; set; }

        [ConfigMeta(Name = "ASSET_GITHUB_REPO_NAME", Required = true)]
        public string AssetGitHubRepoName { get; set; }

        [ConfigMeta(Name = "ASSET_GITHUB_HOOK_SECRET", Required = true)]
        public string AssetGitHubHookSecret { get; set; }

        [ConfigMeta(Name = "STARTUP_CLONE_ASSETS", DefaultValue = true)]
        public bool CloneAssetsDuringStartup { get; set; }

        [ConfigMeta(Name = "ASSET_LOCAL_COMMIT_USER_NAME", DefaultValue = "blog-system")]
        public string AssetGitCommitUser { get; set; }

        [ConfigMeta(Name = "ASSET_LOCAL_COMMIT_USER_EMAIL", DefaultValue = "blog@laobian.me")]
        public string AssetGitCommitEmail { get; set; }

        [ConfigMeta(Name = "BLOG_ADDRESS", DefaultValue = "https://blog.laobian.me")]
        public string BlogAddress { get; set; }

        [ConfigMeta(Name = "ASSET_UPDATE_AT_HOUR", DefaultValue = 0)]
        public int AssetUpdateAtHour { get; set; }

        [ConfigMeta(Name = "POST_GIT_PATH", DefaultValue = "blog/post/")]
        public string PostGitPath { get; set; }

        [ConfigMeta(Name = "CATEGORY_GIT_PATH", DefaultValue = "blog/category.txt")]
        public string CategoryGitPath { get; set; }

        [ConfigMeta(Name = "TAG_GIT_PATH", DefaultValue = "blog/tag.txt")]
        public string TagGitPath { get; set; }

        [ConfigMeta(Name = "ABOUT_GIT_PATH", DefaultValue = "blog/about.md")]
        public string AboutGitPath { get; set; }

<<<<<<< HEAD
        [ConfigMeta(Name = "POST_METADATA_PATH", DefaultValue = "blog/_db/post-metadata.json")]
        public string PostMetadataPath { get; set; }
=======
        [ConfigMeta(Name = "POST_ACCESS_GIT_PATH", DefaultValue = "blog/post-access.txt")]
        public string PostAccessGitPath { get; set; }
>>>>>>> master

        [ConfigMeta(Name = "FILE_GIT_PATH", DefaultValue = "blog/file/")]
        public string FileGitPath { get; set; }

        [ConfigMeta(Name = "FILE_REQUEST_PATH", DefaultValue = "/static")]
        public string FileRequestPath { get; set; }

        [ConfigMeta(Name = "BLOG_DESCRIPTION", DefaultValue = "技术心得与生活感悟 - Jerry Bian(卞良忠)")]
        public string Description { get; set; }

        [ConfigMeta(Name = "BLOG_NAME", DefaultValue = "Jerry Bian's blog")]
        public string Name { get; set; }

        [ConfigMeta(Name = "POSTS_PER_PAGE", DefaultValue = 8)]
        public int PostsPerPage { get; set; }

        [ConfigMeta(Name = "LOG_FLUSH_AT_HOUR", DefaultValue = 9)]
        public int LogFlushAtHour { get; set; }

        [ConfigMeta(Name = "WARNING_LOG_THRESHOLD", DefaultValue = 100)]
        public int WarningLogsThreshold { get; set; }

        [ConfigMeta(Name = "ERROR_LOG_THRESHOLD", DefaultValue = 10)]
        public int ErrorLogsThreshold { get; set; }

        [ConfigMeta(Name = "CRITICAL_LOG_THRESHOLD", DefaultValue = 1)]
        public int CriticalLogsThreshold { get; set; }
    }
}