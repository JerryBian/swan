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
using Polly;
using Polly.Extensions.Http;

namespace Laobian.Read
{
    public class Startup
    {
        private readonly IHostEnvironment _env;

        public Startup(IConfiguration configuration, IHostEnvironment env)
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
            var option = new ReadOption();
            var resolver = new ReadOptionResolver();
            resolver.Resolve(option, Configuration);
            services.Configure<ReadOption>(o => { o.Clone(option); });
            StartupHelper.ConfigureServices(services, _env, option);
            services.AddSingleton<ILaobianLogQueue, LaobianLogQueue>();
            services.AddHttpClient<ApiSiteHttpClient>(h =>
                {
                    h.Timeout = TimeSpan.FromMinutes(10);
                    h.DefaultRequestHeaders.Add(Constants.ApiRequestHeaderToken, option.HttpRequestToken);
                })
                .SetHandlerLifetime(TimeSpan.FromDays(1))
                .AddPolicyHandler(GetRetryPolicy());
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
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
