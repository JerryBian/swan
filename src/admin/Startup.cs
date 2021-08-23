using System;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using Laobian.Admin.HostedService;
using Laobian.Admin.HttpClients;
using Laobian.Share;
using Laobian.Share.Converter;
using Laobian.Share.Extension;
using Laobian.Share.Logger;
using Laobian.Share.Logger.Remote;
using Laobian.Share.Notify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace Laobian.Admin
{
    public class Startup : SharedStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
            Site = LaobianSite.Admin;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.Configure<LaobianAdminOption>(o => { o.FetchFromEnv(Configuration); });

            var httpRequestToken = Configuration.GetValue<string>(Constants.EnvHttpRequestToken);
            services.AddHttpClient<ApiSiteHttpClient>(x =>
            {
                x.DefaultRequestHeaders.Add(Constants.ApiRequestHeaderToken, httpRequestToken);
            }).SetHandlerLifetime(TimeSpan.FromDays(1))
                .AddPolicyHandler(GetHttpClientRetryPolicy());

            services.AddHttpClient<BlogSiteHttpClient>(x =>
            {
                x.DefaultRequestHeaders.Add(Constants.ApiRequestHeaderToken, httpRequestToken);
            }).SetHandlerLifetime(TimeSpan.FromDays(1))
                .AddPolicyHandler(GetHttpClientRetryPolicy());

            services.AddLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Trace);
                config.AddDebug();
                config.AddConsole();
                config.AddRemote(c => { c.LoggerName = "admin"; });
            });

            services.AddHostedService<RemoteLogHostedService>();
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
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime)
        {
            var config = app.ApplicationServices.GetRequiredService<IOptions<LaobianAdminOption>>().Value;
            Configure(app, appLifetime, config);

            app.UseStatusCodePages();
            var fileContentTypeProvider = new FileExtensionContentTypeProvider();
            fileContentTypeProvider.Mappings[".webmanifest"] = "application/manifest+json";
            app.UseStaticFiles(new StaticFileOptions {ContentTypeProvider = fileContentTypeProvider});

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