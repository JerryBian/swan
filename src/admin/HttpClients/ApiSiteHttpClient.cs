using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Extension;
using Laobian.Share.Logger;
using Laobian.Share.Site.Blog;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Site.Read;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.HttpClients
{
    public class ApiSiteHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiSiteHttpClient> _logger;

        public ApiSiteHttpClient(HttpClient httpClient, ILogger<ApiSiteHttpClient> logger,
            IOptions<LaobianAdminOption> config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(config.Value.ApiLocalEndpoint);
        }

        public async Task<bool> PersistentAsync(string message)
        {
            var response = await _httpClient.PostAsync("/blog/persistent",
                new StringContent(message, Encoding.UTF8, MediaTypeNames.Text.Plain));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(PersistentAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return false;
            }

            return true;
        }

        public async Task<List<BlogPostRuntime>> GetPostsAsync()
        {
            var response = await _httpClient.GetAsync("/blog/post");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(GetPostsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return new List<BlogPostRuntime>();
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<List<BlogPostRuntime>>(stream);
        }

        public async Task<BlogPostRuntime> GetPostAsync(string link)
        {
            var response = await _httpClient.GetAsync($"/blog/post/{link}");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(GetPostAsync)}({link}) failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<BlogPostRuntime>(stream);
        }

        public async Task AddPostAsync(BlogPost post)
        {
            var response = await _httpClient.PutAsync("/blog/post",
                new StringContent(JsonUtil.Serialize(post), Encoding.UTF8, MediaTypeNames.Application.Json));
            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(AddPostAsync)} failed. Status: {response.StatusCode}. Content: {content}");
            }
        }

        public async Task UpdatePostAsync(BlogPost post)
        {
            var response = await _httpClient.PostAsync("/blog/post",
                new StringContent(JsonUtil.Serialize(post), Encoding.UTF8, MediaTypeNames.Application.Json));
            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(UpdatePostAsync)} failed. Status: {response.StatusCode}. Content: {content}");
            }
        }

        public async Task PostNewAccessAsync(string link)
        {
            var response = await _httpClient.PostAsync($"/blog/post/access/{link}",
                new StringContent(string.Empty));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(PostNewAccessAsync)}({link}) failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            }
        }

        public async Task<List<BlogTag>> GetTagsAsync()
        {
            var response = await _httpClient.GetAsync("/blog/tag");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(GetTagsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return new List<BlogTag>();
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<List<BlogTag>>(stream);
        }

        public async Task<BlogTag> GetTagAsync(string link)
        {
            var response = await _httpClient.GetAsync($"/blog/tag/{link}");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(GetTagAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
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
                    $"{nameof(ApiSiteHttpClient)}.{nameof(AddTagAsync)} failed. Status: {response.StatusCode}. Content: {content}");
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
                    $"{nameof(ApiSiteHttpClient)}.{nameof(UpdateTagAsync)} failed. Status: {response.StatusCode}. Content: {content}");
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
                    $"{nameof(ApiSiteHttpClient)}.{nameof(DeleteTagAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return false;
            }

            return true;
        }

        public async Task SendLogsAsync(IEnumerable<LaobianLog> logs)
        {
            var response = await _httpClient.PostAsync("/log/admin",
                new StringContent(JsonUtil.Serialize(logs), Encoding.UTF8, MediaTypeNames.Application.Json));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(SendLogsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            }
        }

        public async Task<List<LaobianLog>> GetLogsAsync(string site, int days)
        {
            var response = await _httpClient.GetAsync($"/log/{site}?days={days}");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(GetLogsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return new List<LaobianLog>();
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<List<LaobianLog>>(stream);
        }

        public async Task<List<BookItem>> GetReadItemsAsync()
        {
            var response = await _httpClient.GetAsync("/read");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(GetReadItemsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<List<BookItem>>(stream);
        }

        public async Task<BookItem> GetReadItemAsync(string id)
        {
            var response = await _httpClient.GetAsync($"/read/{id}");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(GetReadItemAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<BookItem>(stream);
        }

        public async Task AddBookItemAsync(BookItem bookItem)
        {
            var response = await _httpClient.PutAsync("/read",
                new StringContent(JsonUtil.Serialize(bookItem), Encoding.UTF8, MediaTypeNames.Application.Json));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(AddBookItemAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            }
        }

        public async Task UpdateBookItemAsync(BookItem bookItem)
        {
            var response = await _httpClient.PostAsync("/read",
                new StringContent(JsonUtil.Serialize(bookItem), Encoding.UTF8, MediaTypeNames.Application.Json));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(UpdateBookItemAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            }
        }

        public async Task DeleteBookItemAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"/read/{id}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(DeleteTagAsync)} failed. Status: {response.StatusCode}. Content: {content}");
            }
        }

        public async Task<string> UploadFileAsync(string fileName, byte[] content)
        {
            var response =
                await _httpClient.PostAsync($"/file/upload?fileName={fileName}", new ByteArrayContent(content));
            var url = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(UploadFileAsync)} failed. Status: {response.StatusCode}. Content: {url}");
                return string.Empty;
            }

            return url;
        }

        public async Task<Diary> GetDiaryAsync(DateTime date)
        {
            var response = await _httpClient.GetAsync($"/jarvis/diary/{date.ToDate()}");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(GetDiaryAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtil.DeserializeAsync<Diary>(stream);
        }

        public async Task AddDiaryAsync(Diary diary)
        {
            var response = await _httpClient.PutAsync("/jarvis/diary",
                new StringContent(JsonUtil.Serialize(diary), Encoding.UTF8, MediaTypeNames.Application.Json));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(AddDiaryAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            }
        }

        public async Task UpdateDiaryAsync(Diary diary)
        {
            var response = await _httpClient.PostAsync("/jarvis/diary",
                new StringContent(JsonUtil.Serialize(diary), Encoding.UTF8, MediaTypeNames.Application.Json));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(
                    $"{nameof(ApiSiteHttpClient)}.{nameof(UpdateDiaryAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}