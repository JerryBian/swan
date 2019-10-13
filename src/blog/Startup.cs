using System;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Blog.Helpers;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Laobian.Blog
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostEnvironment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment HostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));
            StartupHelper.RegisterService(services, Configuration);

            if (HostEnvironment.IsDevelopment())
            {
                services.AddControllersWithViews(SetCacheProfile).AddRazorRuntimeCompilation();
            }
            else
            {
                services.AddControllersWithViews(SetCacheProfile);
            }

            services.AddDirectoryBrowser();
        }

        private static void SetCacheProfile(MvcOptions options)
        {
            options.CacheProfiles.Add(
                "Cache10Sec",
                new CacheProfile
                {
                    Duration = 10,
                    Location = ResponseCacheLocation.Client,
                    VaryByHeader = "Accept-Encoding"
                });
            options.CacheProfiles.Add(
                "Cache1Hour",
                new CacheProfile
                {
                    Duration = 60 * 60,
                    Location = ResponseCacheLocation.Client,
                    VaryByHeader = "Accept-Encoding"
                });
            options.CacheProfiles.Add(
                "Cache1Day",
                new CacheProfile
                {
                    Duration = 60 * 60 * 24,
                    Location = ResponseCacheLocation.Client,
                    VaryByHeader = "Accept-Encoding"
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("zh-cn");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("zh-cn");

            var appConfig = app.ApplicationServices.GetService<IOptions<AppConfig>>().Value;
            var logger = app.ApplicationServices.GetService<ILogger<Startup>>();

            applicationLifetime.ApplicationStarted.Register(() =>
            {
                BlogState.StartAtUtc = DateTime.UtcNow;
                BlogState.IsDevEnvironment = HostEnvironment.IsDevelopment();
                BlogState.IsStageEnvironment = HostEnvironment.IsStaging();
                BlogState.IsProdEnvironment = HostEnvironment.IsProduction();
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            if (HostEnvironment.IsProduction())
            {
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = async context =>
                    {
                        logger.LogWarning(context.Features.Get<IExceptionHandlerFeature>()?.Error, "Request error occurred.");
                        await context.Response.WriteAsync($"Something was wrong! Please contact {BlogConstant.AuthorEmail}.");
                    }
                });
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages(async context =>
            {
                context.HttpContext.Response.ContentType = "text/plain";
                await context.HttpContext.Response.WriteAsync(
                    "Status code page, status code: " +
                    context.HttpContext.Response.StatusCode);
            });

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
                OnPrepareResponse = SetStaticFileCache
            });

            var fileDirFullPath = Path.Combine(appConfig.AssetRepoLocalDir, BlogConstant.FileGitHub);
            Directory.CreateDirectory(fileDirFullPath);
            var fileServerOptions = new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(fileDirFullPath),
                RequestPath = BlogConstant.FileRequestPath,
                EnableDirectoryBrowsing = true,
            };
            fileServerOptions.StaticFileOptions.OnPrepareResponse = SetStaticFileCache;
            app.UseFileServer(fileServerOptions);

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private static void SetStaticFileCache(StaticFileResponseContext ctx)
        {
            const int durationInSeconds = 60 * 60 * 24 * 30;
            ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                "public,max-age=" + durationInSeconds;
        }
    }
}
