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

        [EnvOption("ADMIN_PASSWORD")]
        public string AdminPassword { get; set; }

        [EnvOption("GITHUB_API_TOKEN")]
        public string GitHubApiToken { get; set; }

        [EnvOption("GITHUB_USER_NAME")]
        public string GitHubUserName { get; set; }

        [EnvOption("GITHUB_REPO_NAME")]
        public string GitHubRepoName { get; set; }

        [EnvOption("GITHUB_BRANCH_NAME")]
        public string GitHubBranchName { get; set; }

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
                }
            }
        }
    }
}
