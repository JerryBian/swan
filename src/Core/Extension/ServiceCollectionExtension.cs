using GitStoreDotnet;
using Microsoft.Extensions.Options;
using Swan.Core.Store;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Swan.Core.Extension;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSwanService(this IServiceCollection services)
    {
        services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));

        services.AddOptions<SwanOption>().BindConfiguration("swan");

        services.AddMemoryCache();
        services.AddOutputCache(options =>
        {
            options.DefaultExpirationTimeSpan = TimeSpan.FromMinutes(30);
            options.AddBasePolicy(x => x.Cache().Tag("obj-all"));
        });
        services.AddResponseCaching();
        //services.AddHostedService<MonitorHostedService>();
        //services.AddHostedService<GitFileHostedService>();
        //services.AddHostedService<PageHitHostedService>();

        services.AddGitStore();

        services.AddSingleton<ISwanStore, SwanStore>();
        //services.AddSingleton<ISwanService, SwanService>();
        //services.AddSingleton<ISwanLogService, SwanLogService>();
        //services.AddSingleton<IGitFileLoggerProcessor, GitFileLoggerProcessor>();

        return services;
    }

    public static IApplicationBuilder UseSwanService(this IApplicationBuilder builder)
    {
        // Options post setup
        var generalOption = builder.ApplicationServices.GetRequiredService<IOptions<SwanOption>>();
        var gitStoreOption = builder.ApplicationServices.GetRequiredService<IOptions<GitStoreOption>>();
        gitStoreOption.Value.LocalDirectory = Path.Combine(Path.GetFullPath(generalOption.Value.DataLocation), "asset");

        return builder;
    }
}
