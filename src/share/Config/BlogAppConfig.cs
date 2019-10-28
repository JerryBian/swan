using System;
using System.Collections.Generic;
using System.Text;

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

        [ConfigMeta(Name = "ASSET_LOCAL_COMMIT_USER_NAME", DefaultValue = "bot")]
        public string AssetGitCommitUser { get; set; }

        [ConfigMeta(Name = "ASSET_LOCAL_COMMIT_USER_EMAIL", DefaultValue = "bot@laobian.me")]
        public string AssetGitCommitEmail { get; set; }

        [ConfigMeta(Name = "BLOG_ADDRESS", DefaultValue = "https://blog.laobian.me/")]
        public string BlogAddress { get; set; }

        [ConfigMeta(Name = "POST_UPDATE_SCHEDULED", DefaultValue = false)]
        public bool PostUpdateScheduled { get; set; }

        [ConfigMeta(Name = "POST_UPDATE_AT_HOUR", DefaultValue = 0)]
        public int PostUpdateAtHour { get; set; }

        [ConfigMeta(Name = "POST_UPDATE_EVERY_SECONDS", DefaultValue = 300)]
        public int PostUpdateEverySeconds { get; set; }
    }
}
