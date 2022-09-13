using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Laobian.Lib.Option
{
    public class LaobianOption
    {
        [EnvOption("SKIP_GIT_OPERATIONS")]
        public bool SkipGitOperations { get; set; }

        [EnvOption("ASSET_LOCATION")]
        public string AssetLocation { get; set; }

        [EnvOption("ADMIN_USER_NAME")]
        public string AdminUserName { get; set; }

        [EnvOption("ADMIN_USER_FULL_NAME", Default = "test")]
        public string AdminUserFullName { get; set; }

        [EnvOption("ADMIN_PASSWORD")]
        public string AdminPassword { get; set; }

        [EnvOption("GITHUB_API_TOKEN")]
        public string GitHubApiToken { get; set; }

        [EnvOption("GITHUB_USER_NAME")]
        public string GitHubUserName { get; set; }

        [EnvOption("GITHUB_USER_EMAIL")]
        public string GitHubUserEmail { get; set; }

        [EnvOption("GITHUB_REPO_NAME")]
        public string GitHubRepoName { get; set; }

        [EnvOption("GITHUB_BRANCH_NAME")]
        public string GitHubBranchName { get; set; }

        [EnvOption("BASE_URL", Default = "localhost:5053")]
        public string BaseUrl { get; set; }

        [EnvOption("ADMIN_EMAIL", Default = "test@test.com")]
        public string AdminEmail { get; set; }

        [EnvOption("TITLE", Default = "Blog Title")]
        public string Title { get; set; }

        [EnvOption("DESCRIPTION", Default = "Blog Description")]
        public string Description { get; set; }

        [EnvOption("APP_NAME", Default = "app")]
        public string AppName { get; set; }

        public void FetchFromEnv(IConfiguration configuration)
        {
            foreach (PropertyInfo propertyInfo in GetType().GetProperties())
            {
                EnvOptionAttribute attr = propertyInfo.GetCustomAttribute<EnvOptionAttribute>();
                if (attr != null)
                {
                    string value = configuration.GetValue<string>(attr.EnvName);
                    if (!string.IsNullOrEmpty(value))
                    {
                        object val = Convert.ChangeType(value, propertyInfo.PropertyType);
                        propertyInfo.SetValue(this, val);
                    }
                    else if(!string.IsNullOrEmpty(attr.Default))
                    {
                        object val = Convert.ChangeType(attr.Default, propertyInfo.PropertyType);
                        propertyInfo.SetValue(this, val);
                    }
                }
            }
        }
    }
}
