using System;
using System.Text.Encodings.Web;
using Laobian.Api.Command;
using Laobian.Api.Grpc;
using Laobian.Api.HostedServices;
using Laobian.Api.HttpClients;
using Laobian.Api.Logger;
using Laobian.Api.Repository;
using Laobian.Api.Service;
using Laobian.Api.Source;
using Laobian.Share;
using Laobian.Share.Converter;
using Laobian.Share.Filters;
using Laobian.Share.Site;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProtoBuf.Grpc.Server;

namespace Laobian.Api;

public class Startup : SharedStartup
{
    public Startup(IConfiguration configuration, IHostEnvironment env) : base(configuration, env)
    {
        Site = LaobianSite.Api;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.Configure<ApiOptions>(o => { o.FetchFromEnv(Configuration); });
        services.AddCodeFirstGrpc(o => { o.MaxReceiveMessageSize = 20 * 1024 * 1024; });

        services.AddSingleton<ICommandClient, ProcessCommandClient>();
        services.AddSingleton<IFileRepository, GitFileRepository>();
        services.AddSingleton<IGitFileService, GitFileService>();
        services.AddSingleton<IBlogFileRepository, BlogFileRepository>();
        services.AddSingleton<IBlogFileService, BlogFileService>();
        services.AddSingleton<IReadFileRepository, ReadFileRepository>();
        services.AddSingleton<IReadFileService, ReadFileService>();
        services.AddSingleton<ILogFileRepository, LogFileRepository>();
        services.AddSingleton<ILogFileService, LogFileService>();
        services.AddSingleton<IRawFileRepository, RawFileRepository>();
        services.AddSingleton<IRawFileService, RawFileService>();
        services.AddSingleton<IDiaryFileRepository, DiaryFileRepository>();
        services.AddSingleton<IDiaryFileService, DiaryFileService>();
        services.AddSingleton<INoteFileRepository, NoteFileRepository>();
        services.AddSingleton<INoteFileService, NoteFileService>();

        services.AddHostedService<GitFileLogHostedService>();
        services.AddHostedService<DbDataHostedService>();

        services.AddHttpClient<BlogSiteHttpClient>(SetHttpClient).SetHandlerLifetime(TimeSpan.FromDays(1))
            .AddPolicyHandler(GetHttpClientRetryPolicy());
        services.AddHttpClient<JarvisSiteHttpClient>(SetHttpClient).SetHandlerLifetime(TimeSpan.FromDays(1))
            .AddPolicyHandler(GetHttpClientRetryPolicy());

        services.AddLogging(config =>
        {
            config.SetMinimumLevel(LogLevel.Debug);
            config.AddDebug();
            config.AddConsole();
            config.AddGitFile(c => { c.LoggerName = "api"; });
        });

        var httpRequestToken = Configuration.GetValue<string>(Constants.EnvHttpRequestToken);
        services.AddControllers(o => { o.Filters.Add(new VerifyTokenActionFilter(httpRequestToken)); })
            .AddJsonOptions(config =>
            {
                config.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                var converter = new IsoDateTimeConverter();
                config.JsonSerializerOptions.Converters.Add(converter);
            });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime)
    {
        var config = app.ApplicationServices.GetRequiredService<IOptions<ApiOptions>>().Value;
        Configure(app, appLifetime, config);

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGrpcService<LogGrpcService>();
            endpoints.MapGrpcService<BlogGrpcService>();
            endpoints.MapGrpcService<ReadGrpcService>();
            endpoints.MapGrpcService<FileGrpcService>();
            endpoints.MapGrpcService<DiaryGrpcService>();
            endpoints.MapGrpcService<NoteGrpcService>();
            endpoints.MapGrpcService<MiscGrpcService>();
        });
    }
}