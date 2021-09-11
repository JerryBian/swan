using System;
using System.IO;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Share.Logger;
using Laobian.Share.Notify;
using Laobian.Share.Option;
using Laobian.Share.Site;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;

namespace Laobian.Share
{
    public abstract class SharedStartup
    {
        protected SharedStartup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnv = env;
        }

        public IConfiguration Configuration { get; }

        public IHostEnvironment CurrentEnv { get; }

        public LaobianSite Site { get; set; }

        protected IAsyncPolicy<HttpResponseMessage> GetHttpClientRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }

        protected void SetHttpClient(HttpClient httpClient)
        {
            var httpRequestToken = Configuration.GetValue<string>(Constants.EnvHttpRequestToken);
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            httpClient.DefaultRequestHeaders.Add(Constants.ApiRequestHeaderToken, httpRequestToken);
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));
            services.AddSingleton<IEmailNotify, SendGridEmailNotify>();
            services.AddSingleton<ILaobianLogQueue, LaobianLogQueue>();

            var dpFolder = Configuration.GetValue<string>(Constants.EnvDataProtectionKeyPath);
            Directory.CreateDirectory(dpFolder);
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpFolder))
                .SetApplicationName($"{Constants.ApplicationName}_{CurrentEnv.EnvironmentName}");

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = $".LAOBIAN.AUTH.{CurrentEnv.EnvironmentName}";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.Cookie.HttpOnly = true;
                    options.ReturnUrlParameter = "returnUrl";
                    options.LoginPath = new PathString("/login");
                    options.LogoutPath = new PathString("/logout");
                    options.Cookie.Domain = CurrentEnv.IsDevelopment() ? "localhost" : ".laobian.me";
                });
        }

        protected void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime,
            LaobianSharedOption option)
        {
            var emailNotify = app.ApplicationServices.GetRequiredService<IEmailNotify>();
            appLifetime.ApplicationStarted.Register(async () =>
            {
                if (!CurrentEnv.IsDevelopment())
                {
                    var message = new SendGridEmailMessage
                    {
                        Content = "<p>恭喜，网站已经运行起来了！</p>",
                        Site = Site,
                        Subject = "site started",
                        SendGridApiKey = option.SendGridApiKey,
                        ToEmailAddress = option.AdminEmail,
                        ToName = option.AdminEnglishName
                    };

                    await emailNotify.SendAsync(message);
                }
            });

            appLifetime.ApplicationStopped.Register(async () =>
            {
                if (!CurrentEnv.IsDevelopment())
                {
                    var message = new SendGridEmailMessage
                    {
                        Content = "<p>网站被停掉了，请确认是否是正常行为。</p>",
                        Site = Site,
                        Subject = "site stopped",
                        SendGridApiKey = option.SendGridApiKey,
                        ToEmailAddress = option.AdminEmail,
                        ToName = option.AdminEnglishName
                    };

                    await emailNotify.SendAsync(message);
                }
            });

            if (CurrentEnv.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }
        }
    }
}