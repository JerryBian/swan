using System;
using System.Text.Encodings.Web;
using Laobian.Admin.HostedService;
using Laobian.Admin.HttpClients;
using Laobian.Share;
using Laobian.Share.Converter;
using Laobian.Share.Logger.Remote;
using Laobian.Share.Site;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

            services.AddHttpClient<ApiSiteHttpClient>(SetHttpClient).SetHandlerLifetime(TimeSpan.FromDays(1))
                .AddPolicyHandler(GetHttpClientRetryPolicy());

            services.AddHttpClient<BlogSiteHttpClient>(SetHttpClient).SetHandlerLifetime(TimeSpan.FromDays(1))
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
                //config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
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