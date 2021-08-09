using System;
using System.Text.Encodings.Web;
using Laobian.Api.Command;
using Laobian.Api.HostedServices;
using Laobian.Api.Logger;
using Laobian.Api.Repository;
using Laobian.Api.Service;
using Laobian.Api.SourceProvider;
using Laobian.Share;
using Laobian.Share.Converter;
using Laobian.Share.Extension;
using Laobian.Share.Notify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Laobian.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ApiOption option = null;
            services.Configure<ApiOption>(o =>
            {
                option = o;
                var resolver = new ApiOptionResolver();
                resolver.Resolve(o, Configuration);
            });
            StartupHelper.ConfigureServices(services, option);

            services.AddSingleton<IGitFileLogQueue, GitFileLogQueue>();
            services.AddSingleton<ICommandClient, ProcessCommandClient>();
            services.AddSingleton<IBlogService, BlogService>();
            services.AddSingleton<IBlogPostRepository, BlogPostRepository>();
            services.AddSingleton<IDbRepository, DbRepository>();
            services.AddSingleton<LocalFileSourceProvider>();
            services.AddSingleton<GitHubSourceProvider>();
            services.AddSingleton<ISourceProviderFactory, SourceProviderFactory>();

            services.AddHostedService<BlogApiHostedService>();

            services.AddLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Debug);
                config.AddDebug();
                config.AddConsole();
                config.AddGitFile(c => { c.LoggerName = "api"; });
            });

            services.AddHealthChecks();

            services.AddControllers().AddJsonOptions(config =>
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
            var emailNotify = app.ApplicationServices.GetRequiredService<IEmailNotify>();
            appLifetime.ApplicationStarted.Register(async () =>
            {
                if (!env.IsDevelopment())
                {
                    var message = new NotifyMessage
                    {
                        Content = $"<p>site started at {DateTime.Now.ToChinaDateAndTime()}.</p>",
                        Site = LaobianSite.Api,
                        Subject = "site started"
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
                        Subject = "site stopped"
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