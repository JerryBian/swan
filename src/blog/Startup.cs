using System;
using System.IO;
using System.Text.Encodings.Web;
using Laobian.Blog.Cache;
using Laobian.Blog.HostedServices;
using Laobian.Blog.HttpClients;
using Laobian.Blog.Service;
using Laobian.Share;
using Laobian.Share.Converter;
using Laobian.Share.Filters;
using Laobian.Share.Logger.Remote;
using Laobian.Share.Site;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog;

public class Startup : SharedStartup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
    {
        Site = LaobianSite.Blog;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.Configure<BlogOptions>(o => { o.FetchFromEnv(Configuration); });

        services.AddSingleton<ICacheClient, CacheClient>();
        services.AddSingleton<IBlogService, BlogService>();


        services.AddHttpClient<ApiSiteHttpClient>(SetHttpClient)
            .SetHandlerLifetime(TimeSpan.FromDays(1))
            .AddPolicyHandler(GetHttpClientRetryPolicy());

        services.AddHostedService<RemoteLogHostedService>();
        services.AddHostedService<BlogHostedService>();
        services.AddHostedService<PostAccessHostedService>();

        services.AddLogging(config =>
        {
            config.SetMinimumLevel(LogLevel.Debug);
            config.AddDebug();
            config.AddConsole();
            config.AddRemote(c => { c.LoggerName = "blog"; });
        });

        var httpRequestToken = Configuration.GetValue<string>(Constants.EnvHttpRequestToken);
        services.AddControllersWithViews(option =>
        {
            option.Filters.Add(new VerifyTokenActionFilter(httpRequestToken, new[] {"/api"}));
            option.CacheProfiles.Add(Constants.CacheProfileName, new CacheProfile
            {
                Duration = (int) TimeSpan.FromMinutes(1).TotalSeconds,
                Location = ResponseCacheLocation.Client,
                VaryByHeader = "User-Agent"
            });
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
        var config = app.ApplicationServices.GetRequiredService<IOptions<BlogOptions>>().Value;
        Configure(app, appLifetime, config);

        app.UseStatusCodePages();

        var fileContentTypeProvider = new FileExtensionContentTypeProvider();
        fileContentTypeProvider.Mappings[".webmanifest"] = "application/manifest+json";
        app.UseStaticFiles(new StaticFileOptions {ContentTypeProvider = fileContentTypeProvider});

        if (env.IsDevelopment())
        {
            var dir = Path.Combine(config.AssetLocation, Constants.AssetDbFileFolder);
            Directory.CreateDirectory(dir);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.GetFullPath(dir)),
                RequestPath = ""
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