using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
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
using Swan.Middlewares;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using static System.Net.Mime.MediaTypeNames;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("ENV_");
builder.WebHost.CaptureStartupErrors(true);
builder.WebHost.UseShutdownTimeout(TimeSpan.FromMinutes(5));

builder.Logging.ClearProviders();
LogLevel minLogLevel = builder.Environment.IsProduction() ? LogLevel.Information : LogLevel.Trace;

builder.Logging.AddDebug();
builder.Logging.AddConsole();
builder.Logging.AddFile(x =>
{
    x.MinLogLevel = minLogLevel;
});

// Add services to the container.
builder.Services.Configure<SwanOption>(o => { o.FetchFromEnv(builder.Configuration); });

string assetLoc = builder.Configuration.GetValue<string>("ASSET_LOCATION");
string dpFolder = Path.Combine(assetLoc, "dp", builder.Environment.EnvironmentName);
Directory.CreateDirectory(dpFolder);
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpFolder))
    .SetApplicationName($"APP_{builder.Environment.EnvironmentName}");

builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));

builder.Services.AddSingleton<ICacheClient, MemoryCacheClient>();
builder.Services.AddSingleton<ICommandClient, CommandClient>();
builder.Services.AddSingleton<IFileLoggerProcessor, FileLoggerProcessor>();
builder.Services.AddSingleton<IMemoryObjectStore, MemoryObjectStore>();
builder.Services.AddSingleton<IBlacklistStore, BlacklistStore>();

builder.Services.AddSingleton<IBlogService, BlogService>();
builder.Services.AddSingleton<IReadService, ReadService>();
builder.Services.AddSingleton<ILogService, LogService>();
builder.Services.AddSingleton<IBlogPostAccessService, BlogPostAccessService>();

builder.Services.AddSingleton<IFileObjectStore<BlogPostObject>>
    (x => new FileObjectStore<BlogPostObject>(x.GetRequiredService<IOptions<SwanOption>>(), Constants.Asset.BlogPostDir, Constants.Misc.JsonFileFilter));
builder.Services.AddSingleton<IFileObjectStore<BlogTagObject>>
    (x => new FileObjectStore<BlogTagObject>(x.GetRequiredService<IOptions<SwanOption>>(), Constants.Asset.BlogTagDir, Constants.Asset.BlogTagFile));
builder.Services.AddSingleton<IFileObjectStore<BlogSeriesObject>>
    (x => new FileObjectStore<BlogSeriesObject>(x.GetRequiredService<IOptions<SwanOption>>(), Constants.Asset.BlogSeriesDir, Constants.Asset.BlogSeriesFile));
builder.Services.AddSingleton<IFileObjectStore<ReadObject>>
    (x => new FileObjectStore<ReadObject>(x.GetRequiredService<IOptions<SwanOption>>(), Constants.Asset.ReadDir, Constants.Misc.JsonFileFilter));
builder.Services.AddSingleton<IFileObjectStore<LogObject>>
    (x => new FileObjectStore<LogObject>(x.GetRequiredService<IOptions<SwanOption>>(), Constants.Asset.LogDir, Constants.Asset.LogFile));
builder.Services.AddSingleton<IFileObjectStore<BlogPostAccessObject>>
    (x => new FileObjectStore<BlogPostAccessObject>(x.GetRequiredService<IOptions<SwanOption>>(), Constants.Asset.BlogPostAccessDir, Constants.Asset.BlogPostAccessFile));


builder.Services.AddMemoryCache();
builder.Services.AddHostedService<GitFileHostedService>();
builder.Services.AddHostedService<NonProdHostedService>();
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
    _ = app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = Text.Plain;

            IExceptionHandlerPathFeature exceptionHandlerPathFeature =
               context.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature != null)
            {
                ILogger<Program> logger = context.RequestServices.GetService<ILogger<Program>>();
                logger.LogError($"Access URL {exceptionHandlerPathFeature.Path} has error.");
            }

            await context.Response.WriteAsync("An exception was thrown.");
        });
    });
}

app.UseStatusCodePages();
app.UseMiddleware<BlacklistMiddleware>();

FileExtensionContentTypeProvider fileContentTypeProvider = new()
{
    Mappings =
    {
        [".webmanifest"] = "application/manifest+json"
    }
};

app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = fileContentTypeProvider });
SwanOption option = app.Services.GetService<IOptions<SwanOption>>().Value;
string dir = Path.Combine(option.AssetLocation, Constants.Asset.BaseDir, Constants.Asset.FileDir);
Directory.CreateDirectory(dir);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.GetFullPath(dir)),
    RequestPath = $"/{Constants.Misc.RouterFile}",
    OnPrepareResponse = context =>
    {
        if (!app.Environment.IsDevelopment())
        {
            Microsoft.AspNetCore.Http.Headers.ResponseHeaders headers = context.Context.Response.GetTypedHeaders();
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