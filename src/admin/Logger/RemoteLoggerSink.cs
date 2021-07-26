using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Helper;
using Laobian.Share.Logger;
using Laobian.Share.Logger.Remote;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Logger
{
    public class RemoteLoggerSink : IRemoteLoggerSink
    {
        private readonly AdminConfig _config;

        private readonly IServiceProvider _serviceProvider;

        public RemoteLoggerSink(IServiceProvider serviceProvider, IOptions<AdminConfig> config)
        {
            _config = config.Value;
            _serviceProvider = serviceProvider;
        }

        public async Task SendAsync(string loggerName, IEnumerable<LaobianLog> logs)
        {
            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("log");
            httpClient.BaseAddress = new Uri(_config.ApiLocalEndpoint);

            var response = await httpClient.PostAsync($"/log/{loggerName}",
                new StringContent(JsonHelper.Serialize(logs), Encoding.UTF8, "application/json"));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(
                    $"{nameof(RemoteLoggerSink)}.{nameof(SendAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}