using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using Laobian.Api.Command;
using Laobian.Api.Filter;
using Laobian.Api.HostedServices;
using Laobian.Api.HttpService;
using Laobian.Api.Logger;
using Laobian.Api.Repository;
using Laobian.Api.Service;
using Laobian.Api.SourceProvider;
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
            var option = new ApiOption();
            var resolver = new ApiOptionResolver();
            resolver.Resolve(option, Configuration);
            services.Configure<ApiOption>(o => { o.Clone(option); });
            StartupHelper.ConfigureServices(services, _env, option);

            services.AddSingleton<ILaobianLogQueue, LaobianLogQueue>();
            services.AddSingleton<ICommandClient, ProcessCommandClient>();
            services.AddSingleton<IBlogService, BlogService>();
            services.AddSingleton<IBlogPostRepository, BlogPostRepository>();
            services.AddSingleton<IDbRepository, DbRepository>();
            services.AddSingleton<LocalFileSourceProvider>();
            services.AddSingleton<GitHubSourceProvider>();
            services.AddSingleton<ISourceProviderFactory, SourceProviderFactory>();
            services.AddSingleton<SystemLocker>();

            services.AddHttpClient<BlogHttpService>(h =>
                {
                    h.Timeout = TimeSpan.FromMinutes(10);
                    h.DefaultRequestHeaders.Add(Constants.ApiRequestHeaderToken, option.HttpRequestToken);
                }).SetHandlerLifetime(TimeSpan.FromDays(1))
                .AddPolicyHandler(GetRetryPolicy());
            services.AddHostedService<BlogApiHostedService>();

            services.AddLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Debug);
                config.AddDebug();
                config.AddConsole();
                config.AddGitFile(c =>
                {
                    c.LoggerName = "api";
                    c.BaseDir = Path.Combine(option.GetDbLocation(), "log");
                });
            });

            services.AddHealthChecks();

            services.AddControllers(o => { o.Filters.Add(new VerifyTokenActionFilter(option.HttpRequestToken)); })
                .AddJsonOptions(config =>
                {
                    config.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    var converter = new IsoDateTimeConverter();
                    config.JsonSerializerOptions.Converters.Add(converter);
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Laobian.Api", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            var config = app.ApplicationServices.GetRequiredService<IOptions<ApiOption>>().Value;
            var emailNotify = app.ApplicationServices.GetRequiredService<IEmailNotify>();
            appLifetime.ApplicationStarted.Register(async () =>
            {
                if (!env.IsDevelopment())
                {
                    var message = new NotifyMessage
                    {
                        Content = $"<p>site started at {DateTime.Now.ToChinaDateAndTime()}.</p>",
                        Site = LaobianSite.Api,
                        Subject = "site started",
                        SendGridApiKey = config.SendGridApiKey,
                        ToEmailAddress = config.AdminEmail,
                        ToName = config.AdminEnglishName
                    };

                    await emailNotify.SendAsync(message);
                }
            });

            appLifetime.ApplicationStopped.Register(async () =>
            {
                if (!env.IsDevelopment())
                {
                    var message = new NotifyMessage
                    {
                        Content = $"<p>site stopped at {DateTime.Now.ToChinaDateAndTime()}.</p>",
                        Site = LaobianSite.Api,
                        Subject = "site stopped",
                        SendGridApiKey = config.SendGridApiKey,
                        ToEmailAddress = config.AdminEmail,
                        ToName = config.AdminEnglishName
                    };

                    await emailNotify.SendAsync(message);
                }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Laobian.Api v1"));
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapHealthChecks("/health"); });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}