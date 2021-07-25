using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Laobian.Blog.HttpService;
using Laobian.Share.Converter;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Laobian.Blog
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
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

            var dpFolder = Configuration.GetValue<string>("DATA_PROTECTION_KEY_PATH");
            Directory.CreateDirectory(dpFolder);
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpFolder))
                .SetApplicationName("LAOBIAN");
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = "LAOBIAN_AUTH";
                    options.Cookie.Domain = _env.IsDevelopment() ? "localhost" : ".laobian.me";
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

            var fileLoc = Path.Combine(config.GetBlogFileLocation());
            if (!Directory.Exists(fileLoc))
            {
                Directory.CreateDirectory(fileLoc);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.GetFullPath(fileLoc)),
                RequestPath = "/file"
            });

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