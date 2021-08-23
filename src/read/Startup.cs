using System;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Read.HostedService;
using Laobian.Read.HttpClients;
using Laobian.Share;
using Laobian.Share.Converter;
using Laobian.Share.Logger;
using Laobian.Share.Logger.Remote;
using Laobian.Share.Notify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace Laobian.Read
{
    public class Startup : SharedStartup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env) : base(configuration, env)
        {
            Site = LaobianSite.Blog;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.Configure<ReadOption>(o => { o.FetchFromEnv(Configuration); });

            var httpRequestToken = Configuration.GetValue<string>(Constants.EnvHttpRequestToken);
            services.AddHttpClient<ApiSiteHttpClient>(h =>
                {
                    h.Timeout = TimeSpan.FromMinutes(10);
                    h.DefaultRequestHeaders.Add(Constants.ApiRequestHeaderToken, httpRequestToken);
                })
                .SetHandlerLifetime(TimeSpan.FromDays(1))
                .AddPolicyHandler(GetHttpClientRetryPolicy());
            services.AddHostedService<RemoteLogHostedService>();
            services.AddLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Debug);
                config.AddDebug();
                config.AddConsole();
                config.AddRemote(c => { c.LoggerName = "read"; });
            });

            services.AddControllersWithViews().AddJsonOptions(config =>
            {
                config.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                var converter = new IsoDateTimeConverter();
                config.JsonSerializerOptions.Converters.Add(converter);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime)
        {
            var option = app.ApplicationServices.GetRequiredService<IOptions<ReadOption>>();
            base.Configure(app, appLifetime, option.Value);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
