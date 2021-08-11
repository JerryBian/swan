using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using Laobian.Blog.Cache;
using Laobian.Blog.Data;
using Laobian.Blog.HostedService;
using Laobian.Blog.HttpService;
using Laobian.Share;
using Laobian.Share.Converter;
using Laobian.Share.Extension;
using Laobian.Share.Logger;
using Laobian.Share.Logger.Remote;
using Laobian.Share.Notify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace Laobian.Blog
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

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var option = new BlogOption();
            var resolver = new BlogOptionResolver();
            resolver.Resolve(option, Configuration);
            services.Configure<BlogOption>(o => { o.Clone(option); });
            StartupHelper.ConfigureServices(services, _env, option);

            services.AddSingleton<ILaobianLogQueue, LaobianLogQueue>();
            services.AddSingleton<ISystemData, SystemData>();
            services.AddSingleton<ICacheClient, CacheClient>();

            services.AddHttpClient<ApiHttpService>(h =>
                {
                    h.Timeout = TimeSpan.FromMinutes(10);
                    h.DefaultRequestHeaders.Add(Constants.ApiRequestHeaderToken, option.HttpRequestToken);
                })
                .SetHandlerLifetime(TimeSpan.FromDays(1))
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHostedService<BlogHostedService>();
            services.AddHostedService<LogHostedService>();

            services.AddLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Debug);
                config.AddDebug();
                config.AddConsole();
                config.AddRemote(c => { c.LoggerName = "blog"; });
            });

            services.AddControllersWithViews().AddJsonOptions(config =>
            {
                config.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                var converter = new IsoDateTimeConverter();
                config.JsonSerializerOptions.Converters.Add(converter);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            var config = app.ApplicationServices.GetRequiredService<IOptions<BlogOption>>().Value;
            var emailNotify = app.ApplicationServices.GetRequiredService<IEmailNotify>();
            appLifetime.ApplicationStarted.Register(async () =>
            {
                if (!env.IsDevelopment())
                {
                    var message = new NotifyMessage
                    {
                        Content = $"<p>site started at {DateTime.Now.ToChinaDateAndTime()}.</p>",
                        Site = LaobianSite.Blog,
                        Subject = "site started",
                        SendGridApiKey = config.SendGridApiKey,
                        ToEmailAddress = config.AdminEmail,
                        ToName = config.AdminEnglishName
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
                        Site = LaobianSite.Blog,
                        Subject = "site stopped",
                        SendGridApiKey = config.SendGridApiKey,
                        ToEmailAddress = config.AdminEmail,
                        ToName = config.AdminEnglishName
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

            var fileContentTypeProvider = new FileExtensionContentTypeProvider();
            fileContentTypeProvider.Mappings[".webmanifest"] = "application/manifest+json";
            app.UseStaticFiles(new StaticFileOptions {ContentTypeProvider = fileContentTypeProvider});

            if (env.IsDevelopment())
            {
                var fileLoc = config.GetBlogFileLocation();
                if (!Directory.Exists(fileLoc))
                {
                    Directory.CreateDirectory(fileLoc);
                }

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.GetFullPath(fileLoc)),
                    RequestPath = "/blog"
                });
            }

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