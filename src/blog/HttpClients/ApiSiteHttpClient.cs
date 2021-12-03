using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Logger;
using Laobian.Share.Site.Blog;
using Laobian.Share.Site.Read;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.HttpClients;

public class ApiSiteHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiSiteHttpClient> _logger;

    public ApiSiteHttpClient(HttpClient httpClient, ILogger<ApiSiteHttpClient> logger,
        IOptions<BlogOptions> config)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(config.Value.ApiLocalEndpoint);
    }

    public async Task<List<ReadItem>> GetBookItemsAsync()
    {
        var response = await _httpClient.GetAsync("/read");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(GetBookItemsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            return new List<ReadItem>();
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<List<ReadItem>>(stream);
    }

    public async Task<List<BlogPostRuntime>> GetPostsAsync()
    {
        var response = await _httpClient.GetAsync("/blog/posts");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(GetPostsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            return new List<BlogPostRuntime>();
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<List<BlogPostRuntime>>(stream);
    }

    public async Task<List<BlogTag>> GetTagsAsync()
    {
        var response = await _httpClient.GetAsync("/blog/tags");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(GetTagsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            return new List<BlogTag>();
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<List<BlogTag>>(stream);
    }

    public async Task<BlogPostRuntime> GetPostAsync(string link)
    {
        var response = await _httpClient.GetAsync($"/blog/posts/{link}");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(GetPostAsync)}({link}) failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<BlogPostRuntime>(stream);
    }

    public async Task AddPostAccess(string link)
    {
        var response = await _httpClient.PostAsync($"/blog/posts/{link}/access", new StringContent(""));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(AddPostAccess)}({link}) failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
    }

    public async Task SendLogsAsync(IEnumerable<LaobianLog> logs)
    {
        var response = await _httpClient.PostAsync("/log/blog",
            new StringContent(JsonUtil.Serialize(logs), Encoding.UTF8, "application/json"));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine(
                $"{nameof(ApiSiteHttpClient)}.{nameof(SendLogsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
    }
}