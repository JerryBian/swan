using System;
using Laobian.Blog.HostedService;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Laobian.Share.Infrastructure.Cache;
using Laobian.Share.Infrastructure.Command;
using Laobian.Share.Infrastructure.Email;
using Laobian.Share.Infrastructure.Git;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Laobian.Blog.Helpers
{
    public class StartupHelper
    {
        public static void RegisterService(IServiceCollection services, IConfiguration config)
        {
            
            services.Configure<AppConfig>(ac => MapConfig(config, ac));

            services.AddSingleton<IMemoryCacheClient, MemoryCacheClient>();
            services.AddSingleton<ICommand, PowerShellCommand>();
            services.AddSingleton<IBlogService, BlogService>();
            services.AddSingleton<IGitClient, GitClient>();
            services.AddSingleton<IEmailClient, SendGridEmailClient>();

            services.AddHostedService<PostHostedService>();
        }

        private static void MapConfig(IConfiguration config, AppConfig ac)
        {
            ac.SendGridApiKey = config.GetValue<string>("SEND_GRID_API_KEY");
            ac.AssetGitHubRepoApiToken = config.GetValue<string>("ASSET_GITHUB_REPO_API_TOKEN");
            ac.AssetGitHubRepoBranch = config.GetValue<string>("ASSET_GITHUB_REPO_BRANCH");
            ac.AssetGitHubRepoOwner = config.GetValue<string>("ASSET_GITHUB_REPO_OWNER");
            ac.AssetGitHubRepoName = config.GetValue<string>("ASSET_GITHUB_REPO_NAME");
            ac.AssetRepoLocalDir = config.GetValue<string>("ASSET_REPO_LOCAL_DIR");
            ac.CloneAssetsDuringStartup = config.GetValue("STARTUP_CLONE_ASSETS", true);
            ac.AssetGitCommitUser = config.GetValue("ASSET_LOCAL_COMMIT_USER_NAME", "bot");
            ac.AssetGitCommitEmail = config.GetValue("ASSET_LOCAL_COMMIT_USER_EMAIL", "bot@laobian.me");
            ac.BlogPostHostingServiceInterval = config.GetValue("BLOG_POST_HOSTING_INTERVAL_IN_SECONDS",
                TimeSpan.FromHours(1).TotalSeconds);
            ac.BlogAddress = config.GetValue("BLOG_ADDRESS", "https://blog.laobian.me/");
            ac.AssetGitHubHookSecret = config.GetValue("ASSET_GITHUB_HOOK_SECRET", "test");
        }
    }
}
