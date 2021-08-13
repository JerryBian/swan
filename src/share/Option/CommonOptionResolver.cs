using Microsoft.Extensions.Configuration;

namespace Laobian.Share.Option
{
    public class CommonOptionResolver
    {
        public virtual void Resolve(CommonOption option, IConfiguration configuration)
        {
            option.BlogLocalEndpoint = configuration.GetValue<string>("BLOG_LOCAL_ENDPOINT");
            option.BlogRemoteEndpoint = configuration.GetValue<string>("BLOG_REMOTE_ENDPOINT");
            option.ApiLocalEndpoint = configuration.GetValue<string>("API_LOCAL_ENDPOINT");
            option.FileRemoteEndpoint = configuration.GetValue<string>("FILE_REMOTE_ENDPOINT");
            option.AdminUserName = configuration.GetValue<string>("ADMIN_USER_NAME");
            option.AdminEmail = configuration.GetValue<string>("ADMIN_EMAIL");
            option.AssetLocation = configuration.GetValue<string>("ASSET_LOCATION");
            option.SendGridApiKey = configuration.GetValue<string>("SEND_GRID_API_KEY");
            option.AdminChineseName = configuration.GetValue<string>("ADMIN_CHINESE_NAME");
            option.AdminEnglishName = configuration.GetValue<string>("ADMIN_ENGLISH_NAME");
            option.DataProtectionKeyPath = configuration.GetValue<string>("DATA_PROTECTION_KEY_PATH");
            option.HttpRequestToken = configuration.GetValue<string>("HTTP_REQUEST_TOKEN");
            option.HomePageEndpoint = configuration.GetValue("HOME_PAGE_ENDPOINT", "https://laobian.me");
        }
    }
}