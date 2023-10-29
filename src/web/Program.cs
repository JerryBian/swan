using GitStoreDotnet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Swan.Core.Converter;
using Swan.Core.Extension;
using Swan.Core.Logger;
using Swan.Core.Option;
using Swan.Web.HostedServices;
using Swan.Web.Middlewares;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using static System.Net.Mime.MediaTypeNames;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("ENV_");
builder.WebHost.CaptureStartupErrors(true);
builder.WebHost.UseShutdownTimeout(TimeSpan.FromMinutes(5));

// Add services to the container.
builder.Services.AddOptions<SwanOption>().BindConfiguration("swan");

var assetLoc = builder.Configuration.GetValue<string>("AssetLocation");
if(assetLoc != null)
{
    var dpFolder = Path.Combine(assetLoc, "dp", builder.Environment.EnvironmentName);
    Directory.CreateDirectory(dpFolder);
    builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpFolder))
        .SetApplicationName($"APP_{builder.Environment.EnvironmentName}");
}

builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddGitFile();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.All;
});

builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));

builder.Services.AddMemoryCache();
builder.Services.AddSwanService();

builder.Services.AddHostedService<MonitorHostedService>();
builder.Services.AddHostedService<GitFileHostedService>();
builder.Services.AddHostedService<PageHitHostedService>();

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

builder.Services.AddControllersWithViews().AddJsonOptions(config =>
{
    config.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    IsoDateTimeConverter converter = new();
    config.JsonSerializerOptions.Converters.Add(converter);
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = Text.Plain;

            var exceptionHandlerPathFeature =
               context.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature != null)
            {
                var logger = context.RequestServices.GetService<ILogger<Program>>();
                logger.LogError(exceptionHandlerPathFeature.Error, $"Access URL {exceptionHandlerPathFeature.Path} has error.");
            }

            await context.Response.WriteAsync("An error was happening.");
        });
    });
}

app.UseForwardedHeaders();
app.UseSwanService();
app.UseStatusCodePages();

FileExtensionContentTypeProvider fileContentTypeProvider = new()
{
    Mappings =
    {
        [".webmanifest"] = "application/manifest+json"
    }
};

app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = fileContentTypeProvider });
GitStoreOption option = app.Services.GetService<IOptions<GitStoreOption>>().Value;
string dir = Path.Combine(option.LocalDirectory, "static");
Directory.CreateDirectory(dir);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.GetFullPath(dir)),
    RequestPath = $"/static",
    OnPrepareResponse = context =>
    {
        if (!app.Environment.IsDevelopment())
        {
            Microsoft.AspNetCore.Http.Headers.ResponseHeaders headers = context.Context.Response.GetTypedHeaders();
            headers.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromDays(30)
            };
        }
    }
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestSniffMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
