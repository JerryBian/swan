using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.HttpService
{
    public class BlogHttpService
    {
        private readonly ApiOption _apiOption;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BlogHttpService> _logger;

        public BlogHttpService(HttpClient httpClient, IOptions<ApiOption> apiConfig, ILogger<BlogHttpService> logger)
        {
            _logger = logger;
            _apiOption = apiConfig.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_apiOption.BlogLocalEndpoint);
        }

        public async Task PurgeCacheAsync()
        {
            var response = await _httpClient.PostAsync("/api/PurgeCache", new StringContent(string.Empty));
            response.EnsureSuccessStatusCode();
        }
    }
}