using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Extension;
using Laobian.Share.Logger;
using Laobian.Share.Site;
using Laobian.Share.Util;

namespace Laobian.Api.Service;

public class LogFileService : ILogFileService
{
    private readonly ILogFileRepository _logFileRepository;

    public LogFileService(ILogFileRepository logFileRepository)
    {
        _logFileRepository = logFileRepository;
    }

    public async Task<List<LaobianLog>> GetLogsAsync(LaobianSite site, DateTime date,
        CancellationToken cancellationToken = default)
    {
        var result = new List<LaobianLog>();
        var logFile =
            (await _logFileRepository.SearchFilesAsync(
                Path.Combine(site.ToString().ToLowerInvariant(), date.Year.ToString("D4"), $"{date.ToDate()}.log"),
                cancellationToken: cancellationToken)).FirstOrDefault();
        if (!string.IsNullOrEmpty(logFile))
        {
            var logs = await _logFileRepository.ReadAsync(logFile, cancellationToken);
            if (!string.IsNullOrEmpty(logs))
            {
                using var sr = new StringReader(logs);
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    result.Add(JsonUtil.Deserialize<LaobianLog>(line));
                }
            }
        }

        return result;
    }

    public async Task AddLogAsync(LaobianLog log, CancellationToken cancellationToken = default)
    {
        var site = LaobianSite.Api;
        if (Enum.TryParse(log.LoggerName, true, out LaobianSite temp))
        {
            site = temp;
        }

        await _logFileRepository.AppendLineAsync(
            Path.Combine(site.ToString().ToLowerInvariant(), log.TimeStamp.Year.ToString("D4"),
                $"{log.TimeStamp.ToDate()}.log"), JsonUtil.Serialize(log), cancellationToken);
    }
}