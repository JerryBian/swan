using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace Laobian.Share.Option;

public class SharedOptions
{
    [OptionEnvName("BLOG_LOCAL_ENDPOINT")] public string BlogLocalEndpoint { get; set; }

    [OptionEnvName("BLOG_REMOTE_ENDPOINT")]
    public string BlogRemoteEndpoint { get; set; }

    [OptionEnvName("API_LOCAL_ENDPOINT")] public string ApiLocalEndpoint { get; set; }

    [OptionEnvName("FILE_REMOTE_ENDPOINT")]
    public string FileRemoteEndpoint { get; set; }

    [OptionEnvName("ADMIN_REMOTE_ENDPOINT")]
    public string AdminRemoteEndpoint { get; set; }

    [OptionEnvName("JARVIS_REMOTE_ENDPOINT")]
    public string JarvisRemoteEndpoint { get; set; }

    [OptionEnvName("ADMIN_USER_NAME")] public string AdminUserName { get; set; }

    [OptionEnvName("ADMIN_EMAIL")] public string AdminEmail { get; set; }

    [OptionEnvName("ADMIN_CHINESE_NAME")] public string AdminChineseName { get; set; }

    [OptionEnvName("ADMIN_ENGLISH_NAME")] public string AdminEnglishName { get; set; }

    [OptionEnvName("ASSET_LOCATION")] public string AssetLocation { get; set; }

    [OptionEnvName("SEND_GRID_API_KEY")] public string SendGridApiKey { get; set; }

    [OptionEnvName(Constants.EnvHttpRequestToken)]
    public string HttpRequestToken { get; set; }

    public string AppVersion
    {
        get
        {
            var ver = Assembly.GetEntryAssembly()?.GetName().Version;
            if (ver == null)
            {
                return "1.0";
            }

            return $"{ver.Major}.{ver.Minor}";
        }
    }

    public string RuntimeVersion => RuntimeInformation.FrameworkDescription;

    [OptionEnvName("HOME_PAGE_ENDPOINT")] public string HomePageEndpoint { get; set; }

    public void FetchFromEnv(IConfiguration configuration)
    {
        foreach (var propertyInfo in GetType().GetProperties())
        {
            var attr = propertyInfo.GetCustomAttribute<OptionEnvNameAttribute>();
            if (attr != null)
            {
                var value = configuration.GetValue<string>(attr.EnvName);
                propertyInfo.SetValue(this, value);
            }
        }
    }
}