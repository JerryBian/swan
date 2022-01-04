using System;
using System.Text.Encodings.Web;
using Laobian.Jarvis.HostedServices;
using Laobian.Jarvis.HttpClients;
using Laobian.Jarvis.Middleware;
using Laobian.Share;
using Laobian.Share.Converter;
using Laobian.Share.Filters;
using Laobian.Share.Logger.Remote;
using Laobian.Share.Misc;
using Laobian.Share.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Jarvis;

public class Startup : SharedStartup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
    {
        Site = LaobianSite.Jarvis;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.Configure<JarvisOptions>(o => { o.FetchFromEnv(Configuration); });

        services.AddHttpClient<ApiSiteHttpClient>(SetHttpClient)
            .SetHandlerLifetime(TimeSpan.FromDays(1))
            .AddPolicyHandler(GetHttpClientRetryPolicy());

        services.AddHostedService<RemoteLogHostedService>();

        services.AddLogging(config =>
        {
            config.SetMinimumLevel(LogLevel.Debug);
            config.AddDebug();
            config.AddConsole();
            config.AddRemote(c => { c.LoggerName = "jarvis"; });
        });

        var httpRequestToken = Configuration.GetValue<string>(Constants.EnvHttpRequestToken);
        services.AddControllersWithViews(config =>
            {
                config.Filters.Add(new VerifyTokenActionFilter(httpRequestToken, new[] {"/api"}));
            })
            .AddJsonOptions(config =>
            {
                config.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                var converter = new IsoDateTimeConverter();
                config.JsonSerializerOptions.Converters.Add(converter);
            });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime)
    {
        var config = app.ApplicationServices.GetRequiredService<IOptions<JarvisOptions>>().Value;
        Configure(app, appLifetime, config);

        app.UseStatusCodePages();
        var fileContentTypeProvider = new FileExtensionContentTypeProvider();
        fileContentTypeProvider.Mappings[".webmanifest"] = "application/manifest+json";
        app.UseStaticFiles(new StaticFileOptions {ContentTypeProvider = fileContentTypeProvider});

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UsePostAuthentication();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}");
        });
    }
}