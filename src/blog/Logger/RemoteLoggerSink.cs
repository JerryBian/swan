using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Logger;
using Laobian.Share.Logger.Remote;
using Laobian.Share.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Logger
{
    public class RemoteLoggerSink : IRemoteLoggerSink
    {
        private readonly BlogOption _option;

        private readonly IServiceProvider _serviceProvider;

        public RemoteLoggerSink(IServiceProvider serviceProvider, IOptions<BlogOption> config)
        {
            _option = config.Value;
            _serviceProvider = serviceProvider;
        }

        public async Task SendAsync(string loggerName, IEnumerable<LaobianLog> logs)
        {
            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("log");
            httpClient.BaseAddress = new Uri(_option.ApiLocalEndpoint);

            var response = await httpClient.PostAsync($"/log/{loggerName}",
                new StringContent(JsonUtil.Serialize(logs), Encoding.UTF8, "application/json"));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(
                    $"{nameof(RemoteLoggerSink)}.{nameof(SendAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}