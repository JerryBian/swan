using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Laobian.Share.Blog;
using Laobian.Share.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.HttpService
{
    public class ApiHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiHttpService> _logger;

        public ApiHttpService(HttpClient httpClient, ILogger<ApiHttpService> logger, IOptions<BlogConfig> config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(config.Value.ApiLocalEndpoint);
        }

        public async Task<List<BlogPost>> GetPostsAsync(bool onlyPublished)
        {
            var response = await _httpClient.GetAsync($"/blog/posts?onlyPublished={onlyPublished}");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(GetPostsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return new List<BlogPost>();
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonHelper.DeserializeAsync<List<BlogPost>>(stream);
        }

        public async Task<List<BlogTag>> GetTagsAsync()
        {
            var response = await _httpClient.GetAsync("/blog/tags");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(GetTagsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return new List<BlogTag>();
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonHelper.DeserializeAsync<List<BlogTag>>(stream);
        }

        public async Task<BlogPost> GetPostAsync(string link)
        {
            var response = await _httpClient.GetAsync($"/blog/post/{link}");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(GetPostAsync)}({link}) failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonHelper.DeserializeAsync<BlogPost>(stream);
        }
    }
}
