using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Site.Read;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Jarvis.HttpClients
{
    public class ApiSiteHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiSiteHttpClient> _logger;

        public ApiSiteHttpClient(HttpClient httpClient, ILogger<ApiSiteHttpClient> logger,
            IOptions<JarvisOption> config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(config.Value.ApiLocalEndpoint);
        }

        public async Task<List<Diary>> GetDiariesAsync()
        {
            var response = await _httpClient.GetAsync("/diary");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(GetDiariesAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return new List<Diary>();
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<List<Diary>>(stream);
        }
    }
}
