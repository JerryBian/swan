using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.HttpService
{
    public class BlogHttpService
    {
        private readonly ApiConfig _apiConfig;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BlogHttpService> _logger;

        public BlogHttpService(HttpClient httpClient, IOptions<ApiConfig> apiConfig, ILogger<BlogHttpService> logger)
        {
            _logger = logger;
            _apiConfig = apiConfig.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_apiConfig.BlogPostLocation);
        }

        public async Task PurgeCacheAsync()
        {
            var response = await _httpClient.PostAsync("/api/PurgeCache", new StringContent(string.Empty));
            response.EnsureSuccessStatusCode();
        }
    }
}
