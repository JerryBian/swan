using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using Laobian.Api._2.Source;
using Laobian.Api.Command;
using Laobian.Api.Filter;
using Laobian.Api.HostedServices;
using Laobian.Api.HttpClients;
using Laobian.Api.Logger;
using Laobian.Api.Repository;
using Laobian.Share;
using Laobian.Share.Converter;
using Laobian.Share.Extension;
using Laobian.Share.Logger;
using Laobian.Share.Notify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;

namespace Laobian.Api
{
    public class Startup : SharedStartup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env) : base(configuration, env)
        {
            Site = LaobianSite.Api;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.Configure<LaobianApiOption>(o => { o.FetchFromEnv(Configuration); });
            services.AddSingleton<ICommandClient, ProcessCommandClient>();
            services.AddSingleton<IFileRepository, FileRepository>();
            if (CurrentEnv.IsDevelopment())
            {
                services.AddSingleton<IFileSource, LocalFileSource>();
            }
            else
            {
                services.AddSingleton<IFileSource, GitFileSource>();
            }

            services.AddHostedService<GitFileLogHostedService>();
            services.AddHostedService<DbDataHostedService>();

            var httpRequestToken = Configuration.GetValue<string>(Constants.EnvHttpRequestToken);
            services.AddHttpClient<BlogSiteHttpClient>(h =>
                {
                    h.Timeout = TimeSpan.FromMinutes(10);
                    h.DefaultRequestHeaders.Add(Constants.ApiRequestHeaderToken, httpRequestToken);
                }).SetHandlerLifetime(TimeSpan.FromDays(1))
                .AddPolicyHandler(GetHttpClientRetryPolicy());

            services.AddLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Debug);
                config.AddDebug();
                config.AddConsole();
                config.AddGitFile(c =>
                {
                    c.LoggerName = "api";
                });
            });

            services.AddControllers(o => { o.Filters.Add(new VerifyTokenActionFilter(httpRequestToken)); })
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
            var config = app.ApplicationServices.GetRequiredService<IOptions<LaobianApiOption>>().Value;
            Configure(app, appLifetime, config);

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}