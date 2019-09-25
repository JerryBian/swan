using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Blog.HostedService;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Laobian.Share.Infrastructure.Cache;
using Laobian.Share.Infrastructure.Command;
using Laobian.Share.Infrastructure.GitHub;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Laobian.Blog
{
    public class BlogServiceRegister
    {
        public static void Register(IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));
            services.Configure<AppConfig>(ac =>
            {
                ac.SendGridApiKey = config.GetValue<string>("SEND_GRID_API_KEY");
                ac.AssetGitHubRepoApiToken = config.GetValue<string>("ASSET_GITHUB_REPO_API_TOKEN");
                ac.AssetGitHubRepoBranch = config.GetValue<string>("ASSET_GITHUB_REPO_BRANCH");
                ac.AssetGitHubRepoOwner = config.GetValue<string>("ASSET_GITHUB_REPO_OWNER");
                ac.AssetGitHubRepoName = config.GetValue<string>("ASSET_GITHUB_REPO_NAME");
                ac.AssetRepoLocalDir = config.GetValue<string>("ASSET_REPO_LOCAL_DIR");
                ac.CloneAssetsDuringStartup = config.GetValue("STARTUP_CLONE_ASSETS", true);
            });

            services.AddSingleton<IMemoryCacheClient, MemoryCacheClient>();
            services.AddSingleton<ICommand, PowerShellCommand>();
            services.AddSingleton<IBlogService, BlogService>();
            services.AddSingleton<IGitHubClient, GitHubClient2>();

            services.AddHostedService<BlogAssetHostedService>();
        }
    }
}
