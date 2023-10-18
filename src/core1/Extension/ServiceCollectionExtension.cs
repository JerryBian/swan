using GitStoreDotnet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swan.Core.Logger;
using Swan.Core.Option;
using Swan.Core.Service;
using Swan.Core.Store;

namespace Swan.Core.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSwanService(this IServiceCollection services)
        {
            services.AddGitStore();

            services.AddSingleton<ISwanStore, SwanStore>();
            services.AddSingleton<ISwanService, SwanService>();
            services.AddSingleton<ISwanLogService, SwanLogService>();
            services.AddSingleton<IGitFileLoggerProcessor, GitFileLoggerProcessor>();

            return services;
        }

        public static IApplicationBuilder UseSwanService(this IApplicationBuilder builder)
        {
            // Options post setup
            var generalOption = builder.ApplicationServices.GetRequiredService<IOptions<GeneralOption>>();
            var gitStoreOption = builder.ApplicationServices.GetRequiredService<IOptions<GitStoreOption>>();
            gitStoreOption.Value.LocalDirectory = Path.Combine(Path.GetFullPath(generalOption.Value.AssetLocation), "data");

            return builder;
        }
    }
}
