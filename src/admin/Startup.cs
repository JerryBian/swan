using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Admin.HttpService;
using Laobian.Admin.Logger;
using Laobian.Share;
using Laobian.Share.Config;
using Laobian.Share.Converter;
using Laobian.Share.Extension;
using Laobian.Share.Logger.Remote;
using Laobian.Share.Notify;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Admin
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));

            services.AddSingleton<IEmailNotify, EmailNotify>();
            services.Configure<CommonConfig>(Configuration);
            services.Configure<AdminConfig>(Configuration);

            services.AddHttpClient<ApiHttpService>();
            services.AddHttpClient<BlogHttpService>();
            services.AddHttpClient("log");

            services.AddSingleton<IRemoteLoggerSink, RemoteLoggerSink>();

            var dpFolder = Configuration.GetValue<string>("DATA_PROTECTION_KEY_PATH");
            Directory.CreateDirectory(dpFolder);
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpFolder))
                .SetApplicationName("LAOBIAN");
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Cookie.Name = "LAOBIAN_AUTH";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.ReturnUrlParameter = "returnUrl";
                options.LoginPath = new PathString("/login");
                options.LogoutPath = new PathString("/logout");
                options.Cookie.Domain = _env.IsDevelopment() ? "localhost" : ".laobian.me";
            });

            services.AddLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Trace);
                config.AddDebug();
                config.AddConsole();
                config.AddRemote(c => { c.LoggerName = "admin"; });
            });

            services.AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddJsonOptions(config =>
            {
                config.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                var converter = new IsoDateTimeConverter();
                config.JsonSerializerOptions.Converters.Add(converter);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            var emailNotify = app.ApplicationServices.GetRequiredService<IEmailNotify>();
            appLifetime.ApplicationStarted.Register(async () =>
            {
                if (!env.IsDevelopment())
                {
                    var message = new NotifyMessage
                    {
                        Content = $"<p>site started at {DateTime.Now.ToChinaDateAndTime()}.</p>",
                        Site = LaobianSite.Admin,
                        Subject = "site started"
                    };

                    await emailNotify.SendAsync(message);
                }
            });

            appLifetime.ApplicationStopped.Register(async () =>
            {
                if (!env.IsDevelopment())
                {
                    var message = new NotifyMessage
                    {
                        Content = $"<p>site stopped at {DateTime.Now.ToChinaDateAndTime()}.</p>",
                        Site = LaobianSite.Admin,
                        Subject = "site stopped"
                    };

                    await emailNotify.SendAsync(message);
                }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStatusCodePages();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}