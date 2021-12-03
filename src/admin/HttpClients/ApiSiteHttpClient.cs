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

namespace Laobian.Admin.HttpClients;

public class ApiSiteHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiSiteHttpClient> _logger;

    public ApiSiteHttpClient(HttpClient httpClient, ILogger<ApiSiteHttpClient> logger,
        IOptions<AdminOptions> config)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(config.Value.ApiLocalEndpoint);
    }

    public async Task<bool> PersistentAsync(string message)
    {
        var response = await _httpClient.PostAsync("/persistent",
            new StringContent(message, Encoding.UTF8, MediaTypeNames.Text.Plain));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(PersistentAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            return false;
        }

        return true;
    }

    public async Task<List<BlogPostRuntime>> GetPostsAsync(bool extractRuntime)
    {
        var response = await _httpClient.GetAsync($"/blog/posts?extractRuntime={extractRuntime}");
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

    public async Task<BlogPost> AddPostAsync(BlogPost post)
    {
        var response = await _httpClient.PostAsync("/blog/posts",
            new StringContent(JsonUtil.Serialize(post), Encoding.UTF8, MediaTypeNames.Application.Json));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var message = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(AddPostAsync)} failed. Status: {response.StatusCode}. Content: {message}");
            throw new Exception(message);
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<BlogPost>(stream);
    }

    public async Task<BlogPost> UpdatePostAsync(BlogPost post, string replacedPostLink)
    {
        var response = await _httpClient.PutAsync($"/blog/posts?replacedPostLink={replacedPostLink}",
            new StringContent(JsonUtil.Serialize(post), Encoding.UTF8, MediaTypeNames.Application.Json));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var message = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(UpdatePostAsync)} failed. Status: {response.StatusCode}. Content: {message}");
            throw new Exception(message);
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<BlogPost>(stream);
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

    public async Task<BlogTag> GetTagAsync(string id)
    {
        var response = await _httpClient.GetAsync($"/blog/tags/{id}");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(GetTagAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<BlogTag>(stream);
    }

    public async Task<BlogTag> AddTagAsync(BlogTag tag)
    {
        var response = await _httpClient.PostAsync("/blog/tags",
            new StringContent(JsonUtil.Serialize(tag), Encoding.UTF8, MediaTypeNames.Application.Json));

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var message = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(AddTagAsync)} failed. Status: {response.StatusCode}. Content: {message}");
            throw new Exception(message);
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<BlogTag>(stream);
    }

    public async Task<BlogTag> UpdateTagAsync(BlogTag tag)
    {
        var response = await _httpClient.PutAsync("/blog/tags",
            new StringContent(JsonUtil.Serialize(tag), Encoding.UTF8, MediaTypeNames.Application.Json));

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(UpdateTagAsync)} failed. Status: {response.StatusCode}. Content: {content}");
            throw new Exception(content);
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<BlogTag>(stream);
    }

    public async Task<bool> DeleteTagAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"/blog/tags/{id}");
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

    public async Task<List<LaobianLog>> GetLogsAsync(string site, int days, int minLevel)
    {
        var response = await _httpClient.GetAsync($"/log/{site}?days={days}&minLevel={minLevel}");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(GetLogsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            return new List<LaobianLog>();
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<List<LaobianLog>>(stream);
    }

    public async Task<List<ReadItem>> GetReadItemsAsync()
    {
        var response = await _httpClient.GetAsync("/read");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(GetReadItemsAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<List<ReadItem>>(stream);
    }

    public async Task<ReadItem> GetReadItemAsync(string id)
    {
        var response = await _httpClient.GetAsync($"/read/{id}");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(GetReadItemAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<ReadItem>(stream);
    }

    public async Task<ReadItem> AddReadItemAsync(ReadItem readItem)
    {
        var response = await _httpClient.PostAsync("/read",
            new StringContent(JsonUtil.Serialize(readItem), Encoding.UTF8, MediaTypeNames.Application.Json));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine(
                $"{nameof(ApiSiteHttpClient)}.{nameof(AddReadItemAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<ReadItem>(stream);
    }

    public async Task<ReadItem> UpdateReadItemAsync(ReadItem readItem)
    {
        var response = await _httpClient.PutAsync("/read",
            new StringContent(JsonUtil.Serialize(readItem), Encoding.UTF8, MediaTypeNames.Application.Json));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine(
                $"{nameof(ApiSiteHttpClient)}.{nameof(UpdateReadItemAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<ReadItem>(stream);
    }

    public async Task DeleteReadItemAsync(string id)
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

    public async Task<DiaryRuntime> GetDiaryAsync(DateTime date)
    {
        var response = await _httpClient.GetAsync($"/diary/{date.ToDate()}");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(GetDiaryAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<DiaryRuntime>(stream);
    }

    public async Task AddDiaryAsync(Diary diary)
    {
        var response = await _httpClient.PutAsync("/diary",
            new StringContent(JsonUtil.Serialize(diary), Encoding.UTF8, MediaTypeNames.Application.Json));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine(
                $"{nameof(ApiSiteHttpClient)}.{nameof(AddDiaryAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
    }

    public async Task UpdateDiaryAsync(Diary diary)
    {
        var response = await _httpClient.PostAsync("/diary",
            new StringContent(JsonUtil.Serialize(diary), Encoding.UTF8, MediaTypeNames.Application.Json));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine(
                $"{nameof(ApiSiteHttpClient)}.{nameof(UpdateDiaryAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
    }

    public async Task<NoteRuntime> GetNoteAsync(string link)
    {
        var response = await _httpClient.GetAsync($"/note/{link}");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError(
                $"{nameof(ApiSiteHttpClient)}.{nameof(GetNoteAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonUtil.DeserializeAsync<NoteRuntime>(stream);
    }

    public async Task AddNoteAsync(Note note)
    {
        var response = await _httpClient.PutAsync("/note",
            new StringContent(JsonUtil.Serialize(note), Encoding.UTF8, MediaTypeNames.Application.Json));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine(
                $"{nameof(ApiSiteHttpClient)}.{nameof(AddNoteAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
    }

    public async Task UpdateNoteAsync(Note note)
    {
        var response = await _httpClient.PostAsync("/note",
            new StringContent(JsonUtil.Serialize(note), Encoding.UTF8, MediaTypeNames.Application.Json));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine(
                $"{nameof(ApiSiteHttpClient)}.{nameof(UpdateNoteAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
    }
}