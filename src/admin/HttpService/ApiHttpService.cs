using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Blog;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.HttpService
{
    public class ApiHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiHttpService> _logger;

        public ApiHttpService(HttpClient httpClient, ILogger<ApiHttpService> logger, IOptions<AdminConfig> config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(config.Value.ApiLocalEndpoint);
        }

        public async Task<bool> ReloadBlogDataAsync(string message)
        {
            var response = await _httpClient.PostAsync("/blog/reload",
                new StringContent(message, Encoding.UTF8, MediaTypeNames.Text.Plain));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(ReloadBlogDataAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return false;
            }

            return true;
        }

        public async Task<bool> PersistentAsync(string message)
        {
            var response = await _httpClient.PostAsync("/blog/persistent",
                new StringContent(message, Encoding.UTF8, MediaTypeNames.Text.Plain));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(PersistentAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return false;
            }

            return true;
        }

        public async Task<List<BlogPost>> GetPostsAsync()
        {
            var response = await _httpClient.GetAsync("/blog/posts");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(GetPostsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return new List<BlogPost>();
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<List<BlogPost>>(stream);
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
            return await JsonUtil.DeserializeAsync<BlogPost>(stream);
        }

        public async Task PostNewAccessAsync(string link)
        {
            var response = await _httpClient.PostAsync($"/blog/post/access/{link}",
                new StringContent(string.Empty));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(PostNewAccessAsync)}({link}) failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            }
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
            return await JsonUtil.DeserializeAsync<List<BlogTag>>(stream);
        }

        public async Task<bool> UpdatePostMetadataAsync(BlogMetadata metadata)
        {
            var response = await _httpClient.PostAsync("/blog/post/metadata",
                new StringContent(JsonUtil.Serialize(metadata), Encoding.UTF8, "application/json"));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(UpdatePostMetadataAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return false;
            }

            await response.Content.ReadAsStringAsync();
            return true;
        }

        public async Task<BlogTag> GetTagAsync(string link)
        {
            var response = await _httpClient.GetAsync($"/blog/tag/{link}");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(GetTagAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<BlogTag>(stream);
        }

        public async Task<bool> AddTagAsync(BlogTag tag)
        {
            var response = await _httpClient.PutAsync("/blog/tag",
                new StringContent(JsonUtil.Serialize(tag), Encoding.UTF8, MediaTypeNames.Application.Json));
            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(AddTagAsync)} failed. Status: {response.StatusCode}. Content: {content}");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateTagAsync(BlogTag tag)
        {
            var response = await _httpClient.PostAsync("/blog/tag",
                new StringContent(JsonUtil.Serialize(tag), Encoding.UTF8, MediaTypeNames.Application.Json));
            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(UpdateTagAsync)} failed. Status: {response.StatusCode}. Content: {content}");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteTagAsync(string link)
        {
            var response = await _httpClient.DeleteAsync($"/blog/tag/{link}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(DeleteTagAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return false;
            }

            return true;
        }

        public async Task<string> GetLogsAsync(LaobianSite site, string level, DateTime date)
        {
            var response = await _httpClient.GetAsync($"/log/{site}?level={level}&date={date:yyyy-MM-dd}");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(GetLogsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}