﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Extension;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Service
{
    public class DiaryFileService : IDiaryFileService
    {
        private readonly ILogger<DiaryFileService> _logger;
        private readonly IDiaryFileRepository _diaryFileRepository;

        public DiaryFileService(ILogger<DiaryFileService> logger, IDiaryFileRepository diaryFileRepository)
        {
            _logger = logger;
            _diaryFileRepository = diaryFileRepository;
        }

        public async Task<List<Diary>> GetDiariesAsync(int offset = 0, int? count = null, int? year = null, int? month = null,
            CancellationToken cancellationToken = default)
        {
            var searchPath = string.Empty;
            if (year != null)
            {
                if (month.HasValue)
                {
                    searchPath = Path.Combine(searchPath, year.Value.ToString("D4"), month.Value.ToString("D2"));
                }
                else
                {
                    searchPath = Path.Combine(searchPath, year.Value.ToString("D4"));
                }
            }

            var diaries = new List<Diary>();
            var diaryFiles = await _diaryFileRepository.SearchAsync("*.json", searchPath, cancellationToken);
            foreach (var diaryFile in count.HasValue? diaryFiles.OrderByDescending(x => x).Skip(offset).Take(count.Value) : diaryFiles.OrderByDescending(x => x).Skip(offset))
            {
                var diaryJson = await _diaryFileRepository.ReadAsync(diaryFile, cancellationToken);
                if (!DateTime.TryParseExact(Path.GetFileNameWithoutExtension(diaryFile), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    _logger.LogWarning($"Diary file is invalid: {diaryFile}");
                    continue;
                }

                if (string.IsNullOrEmpty(diaryJson))
                {
                    _logger.LogWarning($"Diary is empty: {diaryFile}");
                    continue;
                }

                diaries.Add(JsonUtil.Deserialize<Diary>(diaryJson));
            }

            return diaries;
        }

        public async Task<int> GetDiaryCountAsync(int? year = null, int? month = null, CancellationToken cancellationToken = default)
        {
            var count = 0;
            var searchPath = string.Empty;
            if (year != null)
            {
                if (month.HasValue)
                {
                    searchPath = Path.Combine(searchPath, year.Value.ToString("D4"), month.Value.ToString("D2"));
                }
                else
                {
                    searchPath = Path.Combine(searchPath, year.Value.ToString("D4"));
                }
            }
            var diaryFiles = await _diaryFileRepository.SearchAsync("*.json", searchPath, cancellationToken: cancellationToken);
            foreach (var diaryFile in diaryFiles)
            {
                var diaryJson = await _diaryFileRepository.ReadAsync(diaryFile, cancellationToken);
                if (!DateTime.TryParseExact(Path.GetFileNameWithoutExtension(diaryFile), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    _logger.LogWarning($"Diary file is invalid: {diaryFile}");
                    continue;
                }

                if (string.IsNullOrEmpty(diaryJson))
                {
                    _logger.LogWarning($"Diary is empty: {diaryFile}");
                    continue;
                }

                count++;
            }

            return count;
        }

        public async Task<Diary> GetDiaryAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var diaryFile =
                (await _diaryFileRepository.SearchAsync($"{date.ToDate()}.json", cancellationToken: cancellationToken)).FirstOrDefault();
            if (!string.IsNullOrEmpty(diaryFile))
            {
                var diaryJson = await _diaryFileRepository.ReadAsync(diaryFile, cancellationToken);
                if (!string.IsNullOrEmpty(diaryJson))
                {
                    return JsonUtil.Deserialize<Diary>(diaryJson);
                }
            }

            return null;
        }

        public async Task AddDiaryAsync(Diary diary, CancellationToken cancellationToken = default)
        {
            if (diary == null)
            {
                return;
            }

            var existingDiary = await GetDiaryAsync(diary.Date, cancellationToken);
            if (existingDiary != null)
            {
                throw new Exception($"Diary with date({diary.Date.ToDate()}) already exists. Requested diary: {JsonUtil.Serialize(diary)}");
            }

            diary.CreateTime = diary.LastUpdateTime = DateTime.Now;
            await _diaryFileRepository.WriteAsync(
                Path.Combine(diary.Date.Year.ToString("D4"), $"{diary.Date.ToDate()}.json"),
                JsonUtil.Serialize(diary, true), cancellationToken);
        }

        public async Task UpdateDiaryAsync(Diary diary, CancellationToken cancellationToken = default)
        {
            if (diary == null)
            {
                return;
            }

            var existingDiary = await GetDiaryAsync(diary.Date, cancellationToken);
            if (existingDiary == null)
            {
                throw new Exception($"Diary with date({diary.Date.ToDate()}) not exists. Requested diary: {JsonUtil.Serialize(diary)}");
            }

            diary.CreateTime = existingDiary.CreateTime;
            diary.LastUpdateTime = DateTime.Now;
            await _diaryFileRepository.WriteAsync(
                Path.Combine(diary.Date.Year.ToString("D4"), $"{diary.Date.ToDate()}.json"),
                JsonUtil.Serialize(diary, true), cancellationToken);
        }
    }
}