using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.HttpClients
{
    public class BlogSiteHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BlogSiteHttpClient> _logger;

        public BlogSiteHttpClient(HttpClient httpClient, ILogger<BlogSiteHttpClient> logger, IOptions<LaobianApiOption> config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(config.Value.BlogLocalEndpoint);
        }

        public async Task<bool> ReloadBlogDataAsync()
        {
            var response = await _httpClient.PostAsync("/reload",
                new StringContent(string.Empty, Encoding.UTF8, MediaTypeNames.Text.Plain));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(BlogSiteHttpClient)}.{nameof(ReloadBlogDataAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return false;
            }

            return true;
        }
    }
}