using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Share.Notify;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Laobian.Share
{
    public class StartupHelper
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));

            services.AddSingleton<IEmailNotify, EmailNotify>();

            var dpFolder = configuration.GetValue<string>(Constants.DataProtectionKeyPath);
            Directory.CreateDirectory(dpFolder);
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpFolder))
                .SetApplicationName(Constants.ApplicationName);
        }
    }
}