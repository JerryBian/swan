using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Misc;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.HttpClients;

public class BlogSiteHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BlogSiteHttpClient> _logger;

    public BlogSiteHttpClient(HttpClient httpClient, ILogger<BlogSiteHttpClient> logger,
        IOptions<ApiOptions> config)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(config.Value.BlogLocalEndpoint);
    }

    public async Task ReloadBlogDataAsync()
    {
        var response = await _httpClient.PostAsync("/api/cache/reload",
            new StringContent(string.Empty, Encoding.UTF8, MediaTypeNames.Text.Plain));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(BlogSiteHttpClient)}.{nameof(ReloadBlogDataAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
    }

    public async Task<SiteStat> GetSiteStatAsync()
    {
        var response = await _httpClient.PostAsync("/api/stat",
            new StringContent(string.Empty, Encoding.UTF8, MediaTypeNames.Text.Plain));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(BlogSiteHttpClient)}.{nameof(GetSiteStatAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<SiteStat>(stream);
    }

    public async Task ShutdownAsync()
    {
        var response = await _httpClient.PostAsync("/api/shutdown",
            new StringContent(string.Empty, Encoding.UTF8, MediaTypeNames.Text.Plain));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(BlogSiteHttpClient)}.{nameof(ShutdownAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
    }
}