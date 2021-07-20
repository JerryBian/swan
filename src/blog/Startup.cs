using System;
using System.Collections;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Blog.HttpService;
using Laobian.Share;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));

            services.AddSingleton<ISystemInfo, SystemInfo>();
            services.AddOptions<BlogConfig>().Bind(Configuration).ValidateDataAnnotations();

            services.AddHttpClient<ApiHttpService>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var config = app.ApplicationServices.GetRequiredService<IOptions<BlogConfig>>().Value;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
            {
                Console.WriteLine($"{environmentVariable.Key}: {environmentVariable.Value}");
                _logger.LogInformation($"{environmentVariable.Key}: {environmentVariable.Value}");
            }

            if (config.FileServerBaseUrl == null || !config.FileServerBaseUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(config.BlogPostLocation))
                {
                    throw new LaobianConfigException(nameof(config.BlogPostLocation));
                }

                var fileLoc = Path.Combine(config.BlogPostLocation, "file");
                if (!Directory.Exists(fileLoc))
                {
                    throw new DirectoryNotFoundException($"Directory not exist: {fileLoc}");
                }

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.GetFullPath(fileLoc)),
                    RequestPath = ""
                });
            }
            

            app.UseRouting();

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