using GitStoreDotnet;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Swan.Core;
using Swan.Core.Converter;
using Swan.Core.Extension;
using Swan.Core.Logger;
using Swan.Core.Option;
using Swan.Web.HostedServices;
using System.Text.Encodings.Web;
using System.Text.Unicode;

//await DataHelper.RunAsync();
//return;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("ENV_");
builder.WebHost.CaptureStartupErrors(true);
builder.WebHost.UseShutdownTimeout(TimeSpan.FromMinutes(5));

// Add services to the container.
builder.Services.AddOptions<GeneralOption>().BindConfiguration("General");
builder.Services.AddOptions<BlogOption>().BindConfiguration("Blog");
builder.Services.AddOptions<ReadOption>().BindConfiguration("Read");

builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddGitFile();

builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));

builder.Services.AddMemoryCache();
builder.Services.AddSwanService();

builder.Services.AddHostedService<GitFileHostedService>();

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
    app.UseExceptionHandler("/Home/Error");
}

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
string dir = Path.Combine(option.LocalDirectory, Constants.DataStatic);
Directory.CreateDirectory(dir);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.GetFullPath(dir)),
    RequestPath = $"/{Constants.DataStatic}",
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
