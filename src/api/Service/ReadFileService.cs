using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Site.Read;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Service;

public class ReadFileService : IReadFileService
{
    private readonly ILogger<ReadFileService> _logger;
    private readonly IReadFileRepository _readFileRepository;

    public ReadFileService(ILogger<ReadFileService> logger, IReadFileRepository readFileRepository)
    {
        _logger = logger;
        _readFileRepository = readFileRepository;
    }

    public async Task<List<ReadItem>> GetReadItemsAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<ReadItem>();
        var readFiles = await _readFileRepository.SearchAsync("*.json", cancellationToken: cancellationToken);
        foreach (var readFile in readFiles)
        {
            var readJson = await _readFileRepository.ReadAsync(readFile, cancellationToken);
            if (!string.IsNullOrEmpty(readJson))
            {
                result.AddRange(JsonUtil.Deserialize<IEnumerable<ReadItem>>(readJson));
            }
        }

        return result;
    }

    public async Task<List<ReadItem>> GetReadItemsAsync(int year, CancellationToken cancellationToken = default)
    {
        var readFile =
            (await _readFileRepository.SearchAsync($"{year:D4}.json",
                cancellationToken: cancellationToken)).FirstOrDefault();
        if (!string.IsNullOrEmpty(readFile))
        {
            var readJson = await _readFileRepository.ReadAsync(readFile, cancellationToken);
            if (!string.IsNullOrEmpty(readJson))
            {
                return JsonUtil.Deserialize<List<ReadItem>>(readJson);
            }
        }

        return null;
    }

    public async Task AddReadItemAsync(ReadItem readItem, CancellationToken cancellationToken = default)
    {
        if (readItem == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(readItem.Id))
        {
            readItem.Id = StringUtil.GenerateRandom();
        }

        var existingReadItems =
            await GetReadItemsAsync(readItem.StartTime.Year, cancellationToken) ?? new List<ReadItem>();
        var allReadItems = await GetReadItemsAsync(cancellationToken);
        if (allReadItems.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Id, readItem.Id)) != null)
        {
            throw new Exception($"ReadItem with Id \"{readItem.Id}\" already exists.");
        }

        if (allReadItems.FirstOrDefault(x => x.BookName == readItem.BookName) != null)
        {
            _logger.LogWarning(
                $"It appears you already added same book before: {readItem.BookName}, however it's allowed.");
        }

        readItem.LastUpdateTime = DateTime.Now;
        existingReadItems.Add(readItem);
        await _readFileRepository.WriteAsync($"{readItem.StartTime.Year:D4}.json",
            JsonUtil.Serialize(existingReadItems, true), cancellationToken);
    }

    public async Task UpdateReadItemAsync(ReadItem readItem, CancellationToken cancellationToken = default)
    {
        if (readItem == null)
        {
            return;
        }

        var existingReadItems = await GetReadItemsAsync(readItem.StartTime.Year, cancellationToken);
        if (existingReadItems == null)
        {
            throw new Exception($"ReadItems at year \"{readItem.StartTime.Year}\" not exist.");
        }

        var existingBookItem = existingReadItems.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Id, readItem.Id));
        if (existingBookItem == null)
        {
            throw new Exception($"ReadItem with Id \"{readItem.Id}\" not exist.");
        }

        readItem.LastUpdateTime = DateTime.Now;
        existingReadItems.Remove(existingBookItem);
        existingReadItems.Add(readItem);
        await _readFileRepository.WriteAsync($"{readItem.StartTime.Year:D4}.json",
            JsonUtil.Serialize(existingReadItems, true), cancellationToken);
    }

    public async Task DeleteReadItemAsync(string id, CancellationToken cancellationToken = default)
    {
        var allReadItems = await GetReadItemsAsync(cancellationToken);
        var readItem = allReadItems.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Id, id));
        if (readItem != null)
        {
            var existingReadItems = await GetReadItemsAsync(readItem.StartTime.Year, cancellationToken);
            if (existingReadItems != null)
            {
                var existingBookItem = existingReadItems.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Id, id));
                if (existingBookItem != null)
                {
                    existingReadItems.Remove(existingBookItem);
                    await _readFileRepository.WriteAsync($"{readItem.StartTime.Year:D4}.json",
                        JsonUtil.Serialize(existingReadItems, true), cancellationToken);
                }
            }
        }
    }
}