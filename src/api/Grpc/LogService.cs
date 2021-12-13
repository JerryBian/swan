using System;
using System.Collections.Generic;
using System.Linq;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using Laobian.Share.Grpc.Service;
using ProtoBuf.Grpc;
using System.Threading.Tasks;
using Laobian.Api.Controllers;
using Laobian.Api.Repository;
using Laobian.Share.Logger;
using Laobian.Share.Site;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Grpc
{
    public class LogService : ILogService
    {
        private readonly IFileRepository _fileRepository;
        private readonly ILaobianLogQueue _laobianLogQueue;
        private readonly ILogger<LogService> _logger;

        public LogService(ILogger<LogService> logger, ILaobianLogQueue laobianLogQueue,
            IFileRepository fileRepository)
        {
            _logger = logger;
            _fileRepository = fileRepository;
            _laobianLogQueue = laobianLogQueue;
        }

        public async Task<LogResponse> AddLogsAsync(LogRequest request, CallContext context = default)
        {
            var response = new LogResponse();
            try
            {
                foreach (var log in request.Logs)
                {
                    log.LoggerName = request.Logger;
                    _laobianLogQueue.Add(log);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(LogService)}({nameof(AddLogsAsync)}) failed.");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return await Task.FromResult(response);
        }

        public async Task<LogResponse> GetLogsAsync(LogRequest request, CallContext context = default)
        {
            var response = new LogResponse();
            try
            {
                var logs = new List<LaobianLog>();
                var days = request.Days;
                var minLevel = request.MinLevel;
                if (Enum.TryParse(request.Logger, true, out LaobianSite laobianSite))
                {
                    if (laobianSite == LaobianSite.All)
                    {
                        logs.AddRange(await ReadLogsAsync(LaobianSite.Admin, days, minLevel));
                        logs.AddRange(await ReadLogsAsync(LaobianSite.Blog, days, minLevel));
                        logs.AddRange(await ReadLogsAsync(LaobianSite.Api, days, minLevel));
                        logs.AddRange(await ReadLogsAsync(LaobianSite.Jarvis, days, minLevel));
                    }
                    else
                    {
                        logs.AddRange(await ReadLogsAsync(laobianSite, days, minLevel));
                    }
                }

                response.Logs = logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(LogService)}({nameof(GetLogsAsync)}) failed.");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private async Task<List<LaobianLog>> ReadLogsAsync(LaobianSite site, int days, int minLevel)
        {
            var result = new List<LaobianLog>();
            for (var i = 0; i <= days; i++)
            {
                var date = DateTime.Now.AddDays(-i);
                var logs = await _fileRepository.GetLogsAsync(site, date);
                result.AddRange(logs.Where(x => (int)x.Level >= minLevel));
            }

            return result;
        }
    }
}
