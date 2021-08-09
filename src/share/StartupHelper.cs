using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Share.Notify;
using Laobian.Share.Option;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace Laobian.Share
{
    public class StartupHelper
    {
        public static void ConfigureServices(IServiceCollection services, CommonOption option)
        {
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));

            services.AddSingleton<IEmailNotify, EmailNotify>();

            var dpFolder = option.DataProtectionKeyPath;
            Directory.CreateDirectory(dpFolder);
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpFolder))
                .SetApplicationName(Constants.ApplicationName);
        }
    }
}