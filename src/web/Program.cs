using Laobian.Lib;
using Laobian.Lib.Cache;
using Laobian.Lib.Command;
using Laobian.Lib.Converter;
using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Option;
using Laobian.Lib.Repository;
using Laobian.Lib.Service;
using Laobian.Lib.Store;
using Laobian.Lib.Worker;
using Laobian.Web.HostedServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

// testing
//var f1 = "C:\\temp2\\data\\asset\\blog\\post";
//var f2 = "C:\\temp2\\data\\asset\\blog\\post1";
//Directory.CreateDirectory(f2);
//foreach (var f in Directory.EnumerateFiles(f1, "*", SearchOption.AllDirectories))
//{
//    var c = File.ReadAllText(f);
//    var p = JsonHelper.Deserialize<BlogPost>(c);
//    p.Id = StringHelper.Random();
//    var path = Path.Combine(f2, $"{p.Id}.json");
//    File.WriteAllText(path, JsonHelper.Serialize(p, true));
//}

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureAppConfiguration((hostContext, config) => { _ = config.AddEnvironmentVariables("ENV_"); });

builder.Host.ConfigureLogging(l =>
{
    l.ClearProviders();
    if(builder.Environment.IsProduction())
    {
        l.SetMinimumLevel(LogLevel.Information);
    }
    else
    {
        l.SetMinimumLevel(LogLevel.Trace);
        l.AddDebug();
    }

    l.AddConsole();
});

// Add services to the container.
builder.Services.Configure<LaobianOption>(o => { o.FetchFromEnv(builder.Configuration); });

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

builder.Services.AddMemoryCache();
builder.Services.AddHostedService<GitFileHostedService>();
builder.Services.AddHostedService<BlogPostHostedService>();
builder.Services.AddControllersWithViews(option =>
{
    option.CacheProfiles.Add(Constants.CacheProfileName, new CacheProfile
    {
        Duration = (int)TimeSpan.FromMinutes(1).TotalSeconds,
        Location = ResponseCacheLocation.Any,
        VaryByHeader = "User-Agent"
    });
}).AddJsonOptions(config =>
{
    config.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    var converter = new IsoDateTimeConverter();
    config.JsonSerializerOptions.Converters.Add(converter);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = $".LAOBIAN.AUTH.{builder.Environment.EnvironmentName}";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.ReturnUrlParameter = "returnUrl";
                options.LoginPath = new PathString("/login");
                options.LogoutPath = new PathString("/logout");
                options.Cookie.Domain = builder.Environment.IsDevelopment() ? "localhost" : ".laobian.me";
            });

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error");
}
app.UseStatusCodePages();

var fileContentTypeProvider = new FileExtensionContentTypeProvider
{
    Mappings =
            {
                [".webmanifest"] = "application/manifest+json"
            }
};
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = fileContentTypeProvider });
var option = app.Services.GetService<IOptions<LaobianOption>>().Value;
var dir = Path.Combine(option.AssetLocation, Constants.FolderAsset, Constants.FolderFile);
Directory.CreateDirectory(dir);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.GetFullPath(dir)),
    RequestPath = $"/{Constants.RouterFile}"
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(Constants.AreaAdmin, Constants.AreaAdmin, "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapAreaControllerRoute(Constants.AreaRead, Constants.AreaRead, "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapAreaControllerRoute(Constants.AreaBlog, Constants.AreaBlog, "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.Run();
