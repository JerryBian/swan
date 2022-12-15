using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Swan.HostedServices;
using Swan.Lib;
using Swan.Lib.Cache;
using Swan.Lib.Command;
using Swan.Lib.Converter;
using Swan.Lib.Log;
using Swan.Lib.Option;
using Swan.Lib.Repository;
using Swan.Lib.Service;
using Swan.Lib.Worker;
using Swan.Middlewares;
using System.Text.Encodings.Web;
using System.Text.Unicode;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("ENV_");
builder.WebHost.CaptureStartupErrors(true);
builder.WebHost.UseShutdownTimeout(TimeSpan.FromMinutes(5));

builder.Logging.ClearProviders();
var minLogLevel = builder.Environment.IsProduction() ? LogLevel.Information : LogLevel.Trace;

builder.Logging.AddDebug();
builder.Logging.AddConsole();
builder.Logging.AddFile(x =>
{
    x.MinLogLevel = minLogLevel;
});

// Add services to the container.
builder.Services.Configure<SwanOption>(o => { o.FetchFromEnv(builder.Configuration); });

var assetLoc = builder.Configuration.GetValue<string>("ASSET_LOCATION");
var dpFolder = Path.Combine(assetLoc, "dp", builder.Environment.EnvironmentName);
Directory.CreateDirectory(dpFolder);
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpFolder))
    .SetApplicationName($"SWAN_{builder.Environment.EnvironmentName}");

builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));
builder.Services.AddSingleton<IReadRepository, ReadRepository>();
builder.Services.AddSingleton<IReadService, ReadService>();
builder.Services.AddSingleton<IBlogRepository, BlogRepository>();
builder.Services.AddSingleton<IBlogService, BlogService>();
builder.Services.AddSingleton<ICacheManager, MemoryCacheManager>();
builder.Services.AddSingleton<ICommandClient, CommandClient>();
builder.Services.AddSingleton<IBlogPostAccessWorker, BlogPostAccessWorker>();
builder.Services.AddSingleton<IFileRepository, FileRepository>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<IFileLoggerProcessor, FileLoggerProcessor>();
builder.Services.AddSingleton<ILogRepository, LogRepository>();
builder.Services.AddSingleton<ILogService, LogService>();
builder.Services.AddSingleton<IBlacklistRepository, BlacklistRepository>();
builder.Services.AddSingleton<IBlacklistService, BlacklistService>();

builder.Services.AddMemoryCache();
builder.Services.AddHostedService<GitFileHostedService>();
builder.Services.AddHostedService<BlogPostHostedService>();
builder.Services.AddHostedService<CleanupHostedService>();
builder.Services.AddHostedService<TimerHostedService>();
builder.Services.AddControllersWithViews(option =>
{
    option.CacheProfiles.Add(Constants.CacheProfileClientShort, new CacheProfile
    {
        Duration = (int)TimeSpan.FromMinutes(1).TotalSeconds,
        Location = ResponseCacheLocation.Client,
        VaryByHeader = "User-Agent"
    });

    option.CacheProfiles.Add(Constants.CacheProfileServerShort, new CacheProfile
    {
        Duration = (int)TimeSpan.FromHours(1).TotalSeconds,
        Location = ResponseCacheLocation.Any,
        VaryByHeader = "User-Agent"
    });

    option.CacheProfiles.Add(Constants.CacheProfileServerLong, new CacheProfile
    {
        Duration = (int)TimeSpan.FromDays(1).TotalSeconds,
        Location = ResponseCacheLocation.Any,
        VaryByHeader = "User-Agent"
    });
}).AddJsonOptions(config =>
{
    config.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    IsoDateTimeConverter converter = new();
    config.JsonSerializerOptions.Converters.Add(converter);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = $".SITE.AUTH.{builder.Environment.EnvironmentName}";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.ReturnUrlParameter = "returnUrl";
                options.LoginPath = new PathString("/login");
                options.LogoutPath = new PathString("/logout");
            });

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error");
}
app.UseStatusCodePages();

app.UseMiddleware<BlacklistIpMiddleware>();

FileExtensionContentTypeProvider fileContentTypeProvider = new()
{
    Mappings =
            {
                [".webmanifest"] = "application/manifest+json"
            }
};
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = fileContentTypeProvider });
SwanOption option = app.Services.GetService<IOptions<SwanOption>>().Value;
string dir = Path.Combine(option.AssetLocation, Constants.FolderAsset, Constants.FolderFile);
Directory.CreateDirectory(dir);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.GetFullPath(dir)),
    RequestPath = $"/{Constants.RouterFile}",
    OnPrepareResponse = context =>
    {
        if (!app.Environment.IsDevelopment())
        {
            var headers = context.Context.Response.GetTypedHeaders();
            headers.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromDays(7)
            };
        }
    }
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(Constants.AreaAdmin, Constants.AreaAdmin, "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapAreaControllerRoute(Constants.AreaRead, Constants.AreaRead, "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapAreaControllerRoute(Constants.AreaBlog, Constants.AreaBlog, "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.Run();
