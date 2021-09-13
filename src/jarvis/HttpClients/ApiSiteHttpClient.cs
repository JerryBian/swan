using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Laobian.Share.Extension;
using Laobian.Share.Site.Jarvis;
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

        public async Task<List<DateTime>> ListDiariesAsync(int? year = null, int? month = null)
        {
            var response = await _httpClient.GetAsync($"/diary/list?year={year}&month={month}");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(ListDiariesAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return new List<DateTime>();
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<List<DateTime>>(stream);
        }

        public async Task<DiaryRuntime> GetDiaryAsync(DateTime date)
        {
            var response = await _httpClient.GetAsync($"/diary/{date.ToDate()}");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(ListDiariesAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<DiaryRuntime>(stream);
        }
    }
}