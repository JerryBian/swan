using System;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Blog.Helpers;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
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
                services.AddControllersWithViews().AddRazorRuntimeCompilation();
            }
            else
            {
                services.AddControllersWithViews();
            }

            services.AddDirectoryBrowser();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("zh-cn");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("zh-cn");

            applicationLifetime.ApplicationStarted.Register(() =>
            {
                BlogState.StartAtUtc = DateTime.UtcNow;
                BlogState.IsDevEnvironment = HostEnvironment.IsDevelopment();
                BlogState.IsStageEnvironment = HostEnvironment.IsStaging();
                BlogState.IsProdEnvironment = HostEnvironment.IsProduction();
            });

            var appConfig = app.ApplicationServices.GetService<IOptions<AppConfig>>().Value;

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (HostEnvironment.IsProduction())
            {
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = async context =>
                    {
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

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24 * 30;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        "public,max-age=" + durationInSeconds;
                }
            });

            var fileDirFullPath = Path.Combine(appConfig.AssetRepoLocalDir, BlogConstant.FileGitHub);
            Directory.CreateDirectory(fileDirFullPath);
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(fileDirFullPath),
                RequestPath = BlogConstant.FileRequestPath,
                EnableDirectoryBrowsing = true
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
