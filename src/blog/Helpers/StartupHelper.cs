using System;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Blog.HostedService;
using Laobian.Share;
using Laobian.Share.Blog;
using Laobian.Share.Blog.Alert;
using Laobian.Share.Blog.Asset;
using Laobian.Share.Blog.Parser;
using Laobian.Share.Cache;
using Laobian.Share.Command;
using Laobian.Share.Config;
using Laobian.Share.Email;
using Laobian.Share.Git;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Laobian.Blog.Helpers
{
    public class StartupHelper
    {
        public static void RegisterService(IServiceCollection services, IConfiguration config)
        {
            MapConfig(config);
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));
            services.AddSingleton<ICacheClient, MemoryCacheClient>();
            services.AddSingleton<ICommand, PowerShellCommand>();
            services.AddSingleton<IBlogService, BlogService>();
            services.AddSingleton<IGitClient, GitHubClient>();
            services.AddSingleton<IEmailClient, SendGridEmailClient>();
            services.AddSingleton<IBlogAssetManager, BlogAssetManager>();
            services.AddSingleton<IBlogAlertService, BlogAlertService>();

            services.AddHostedService<AssetHostedService>();
            services.AddHostedService<LogHostedService>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.ReturnUrlParameter = "r";
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                });
        }

        private static void MapConfig(IConfiguration config)
        {
            var ac = new AppConfig();
            foreach (var domainProp in typeof(AppConfig).GetProperties()
                .Where(p => typeof(IConfig).IsAssignableFrom(p.PropertyType)))
            {
                var obj = Activator.CreateInstance(domainProp.PropertyType);
                foreach (var propertyInfo in domainProp.PropertyType.GetProperties())
                {
                    var attr = propertyInfo.GetCustomAttribute<ConfigMetaAttribute>();
                    if (attr != null)
                    {
                        var configValue = config.GetValue(propertyInfo.PropertyType, attr.Name);
                        if (configValue == null)
                        {
                            if (attr.Required)
                            {
                                throw new AppConfigException(
                                    $"Missing AppConfig. Domain: {domainProp.Name}, config: {propertyInfo.Name}.");
                            }

                            configValue = Convert.ChangeType(attr.DefaultValue, propertyInfo.PropertyType);
                        }

                        propertyInfo.SetValue(obj, Convert.ChangeType(configValue, propertyInfo.PropertyType));
                    }
                }

                domainProp.SetValue(ac, obj);
            }

            Global.Config = ac;
        }
    }
}