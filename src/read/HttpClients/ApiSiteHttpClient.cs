using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Laobian.Share.Read;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Read.HttpClients
{
    public class ApiSiteHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiSiteHttpClient> _logger;

        public ApiSiteHttpClient(HttpClient httpClient, ILogger<ApiSiteHttpClient> logger, IOptions<ReadOption> config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(config.Value.ApiLocalEndpoint);
        }

        public async Task<List<BookItem>> GetAllReadItemsAsync()
        {
            var response = await _httpClient.GetAsync("/read");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(GetAllReadItemsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return new List<BookItem>();
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<List<BookItem>>(stream);
        }
    }
}