using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Swan.Core;
using Swan.Core.Cache;
using Swan.Core.Command;
using Swan.Core.Converter;
using Swan.Core.Log;
using Swan.Core.Model.Object;
using Swan.Core.Option;
using Swan.Core.Service;
using Swan.Core.Store;
using Swan.HostedServices;
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
    .SetApplicationName($"APP_{builder.Environment.EnvironmentName}");

builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));

builder.Services.AddSingleton<ICacheClient, MemoryCacheClient>();
builder.Services.AddSingleton<ICommandClient, CommandClient>();
builder.Services.AddSingleton<IFileLoggerProcessor, FileLoggerProcessor>();
builder.Services.AddSingleton<IMemoryObjectStore, MemoryObjectStore>();

builder.Services.AddSingleton<IBlogService, Swan.Core.Service.BlogService>();
builder.Services.AddSingleton<Swan.Core.Service.IReadService, Swan.Core.Service.ReadService>();
builder.Services.AddSingleton<ILogService, LogService>();

builder.Services.AddSingleton<IFileObjectStore<BlogPostObject>>
    (x => new FileObjectStore<BlogPostObject>(x.GetRequiredService<IOptions<SwanOption>>(), Constants.Asset.BlogPostPath, Constants.Misc.JsonFileFilter));
builder.Services.AddSingleton<IFileObjectStore<BlogTagObject>>
    (x => new FileObjectStore<BlogTagObject>(x.GetRequiredService<IOptions<SwanOption>>(), Constants.Asset.BlogTagPath, Constants.Misc.JsonFileFilter));
builder.Services.AddSingleton<IFileObjectStore<BlogSeriesObject>>
    (x => new FileObjectStore<BlogSeriesObject>(x.GetRequiredService<IOptions<SwanOption>>(), Constants.Asset.BlogSeriesPath, Constants.Misc.JsonFileFilter));
builder.Services.AddSingleton<IFileObjectStore<ReadObject>>
    (x => new FileObjectStore<ReadObject>(x.GetRequiredService<IOptions<SwanOption>>(), Constants.Asset.ReadPath, Constants.Misc.JsonFileFilter));
builder.Services.AddSingleton<IFileObjectStore<LogObject>>
    (x => new FileObjectStore<LogObject>(x.GetRequiredService<IOptions<SwanOption>>(), Constants.Asset.LogPath, Constants.Misc.JsonFileFilter));

builder.Services.AddMemoryCache();
builder.Services.AddHostedService<TimerHostedService>();
builder.Services.AddHostedService<GitFileHostedService>();
builder.Services.AddHostedService<AutoShutdownHostedService>();
builder.Services.AddControllersWithViews(option =>
{
    option.CacheProfiles.Add(Constants.Misc.CacheProfileClientShort, new CacheProfile
    {
        Duration = (int)TimeSpan.FromMinutes(1).TotalSeconds,
        Location = ResponseCacheLocation.Client,
        VaryByHeader = "User-Agent"
    });

    option.CacheProfiles.Add(Constants.Misc.CacheProfileServerShort, new CacheProfile
    {
        Duration = (int)TimeSpan.FromHours(1).TotalSeconds,
        Location = ResponseCacheLocation.Any,
        VaryByHeader = "User-Agent"
    });

    option.CacheProfiles.Add(Constants.Misc.CacheProfileServerLong, new CacheProfile
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
                options.Cookie.Name = $".APP.{builder.Environment.EnvironmentName}";
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

FileExtensionContentTypeProvider fileContentTypeProvider = new()
{
    Mappings =
            {
                [".webmanifest"] = "application/manifest+json"
            }
};
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = fileContentTypeProvider });
SwanOption option = app.Services.GetService<IOptions<SwanOption>>().Value;
string dir = Path.Combine(option.AssetLocation, Constants.Asset.BasePath, Constants.Asset.FilePath);
Directory.CreateDirectory(dir);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.GetFullPath(dir)),
    RequestPath = $"/{Constants.Misc.RouterFile}",
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

app.Run();