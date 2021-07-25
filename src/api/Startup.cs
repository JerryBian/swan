using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Api.Command;
using Laobian.Api.HostedServices;
using Laobian.Api.Repository;
using Laobian.Api.Service;
using Laobian.Api.SourceProvider;
using Laobian.Share.Converter;
using Laobian.Share.Logger.File;
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
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));

            services.AddSingleton<ICommandClient, ProcessCommandClient>();
            services.AddSingleton<IBlogService, BlogService>();
            services.AddSingleton<IBlogPostRepository, BlogPostRepository>();
            services.AddSingleton<IDbRepository, DbRepository>();
            services.AddSingleton<LocalFileSourceProvider>();
            services.AddSingleton<GitHubSourceProvider>();
            services.AddSingleton<ISourceProviderFactory, SourceProviderFactory>();

            services.AddHostedService<BlogApiHostedService>();
            services.Configure<ApiConfig>(Configuration);

            services.AddLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Debug);
                config.AddDebug();
                config.AddConsole();
                config.AddGitFile(c => { c.LoggerDir = Configuration.GetValue<string>("ENV_API_LOG_DIR"); });
            });

            services.AddHealthChecks();

            services.AddControllers().AddJsonOptions(config =>
            {
                var converter = new IsoDateTimeConverter();
                config.JsonSerializerOptions.Converters.Add(converter);
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Laobian.Api", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Laobian.Api v1"));
            }

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapHealthChecks("/health"); });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}