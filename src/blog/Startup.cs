using System;
using System.Globalization;
using System.IO;
using Laobian.Blog.Helpers;
using Laobian.Share;
using Laobian.Share.Blog.Alert;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Global.Environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            StartupHelper.RegisterService(services, Configuration);

            var builder = services.AddControllersWithViews();
            if (Global.Environment.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }

            services.AddDirectoryBrowser();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("zh-cn");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("zh-cn");

            var alertService = app.ApplicationServices.GetService<IBlogAlertService>();
            var logger = app.ApplicationServices.GetService<ILogger<Startup>>();

            RegisterEvents(applicationLifetime, alertService, logger);
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            if (Global.Environment.IsProduction())
            {
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = async context =>
                    {
                        logger.LogError(context.Features.Get<IExceptionHandlerFeature>()?.Error,
                            $"Something is wrong! Request Url= {context.Request.Path}");
                        await context.Response.WriteAsync(
                            $"Something was wrong! Please contact {Global.Config.Common.AdminEmail}.");
                    }
                });
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages(async context =>
            {
                logger.LogWarning(
                    $"Hit status code page. Request Url= {context.HttpContext.Request.GetDisplayUrl()}, " +
                    $"Status= {context.HttpContext.Response.StatusCode}, " +
                    $"Request IP= {context.HttpContext.Connection.RemoteIpAddress}, " +
                    $"User Agent={context.HttpContext.Request.Headers["User-Agent"]}.");
                context.HttpContext.Response.ContentType = "text/plain";
                await context.HttpContext.Response.WriteAsync(
                    "Status code page, status code: " +
                    context.HttpContext.Response.StatusCode);
            });

            app.UseStaticFiles();

            var fileDirFullPath = Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.FileGitPath);
            Directory.CreateDirectory(fileDirFullPath);
            var fileServerOptions = new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(fileDirFullPath),
                RequestPath = Global.Config.Blog.FileRequestPath,
                EnableDirectoryBrowsing = true
            };
            app.UseFileServer(fileServerOptions);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }

        private static void RegisterEvents(
            IHostApplicationLifetime applicationLifetime,
            IBlogAlertService alertService,
            ILogger<Startup> logger)
        {
            applicationLifetime.ApplicationStarted.Register(async () =>
            {
                Global.StartTime = DateTime.Now;

                if (!Global.Environment.IsDevelopment())
                {
                    await alertService.AlertEventAsync("Blog started.");
                }

                logger.LogInformation("Application started.");
            });

            applicationLifetime.ApplicationStopping.Register(async () =>
            {
                if (!Global.Environment.IsDevelopment())
                {
                    await alertService.AlertEventAsync("Blog is stopping.");
                }

                logger.LogInformation("Application is stopping.");
            });
        }
    }
}