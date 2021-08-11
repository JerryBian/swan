using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Share.Notify;
using Laobian.Share.Option;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Laobian.Share
{
    public class StartupHelper
    {
        public static void ConfigureServices(IServiceCollection services, IHostEnvironment env, CommonOption option)
        {
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));

            services.AddSingleton<IEmailNotify, EmailNotify>();

            var dpFolder = option.DataProtectionKeyPath;
            Directory.CreateDirectory(dpFolder);
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpFolder))
                .SetApplicationName(Constants.ApplicationName);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "LAOBIAN_AUTH";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.Cookie.HttpOnly = true;
                    options.ReturnUrlParameter = "returnUrl";
                    options.LoginPath = new PathString("/login");
                    options.LogoutPath = new PathString("/logout");
                    options.Cookie.Domain = env.IsDevelopment() ? "localhost" : ".laobian.me";
                });
        }
    }
}