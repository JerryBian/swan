using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Api.Service;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace Laobian.Api.Grpc
{
    public class DiaryGrpcService : IDiaryGrpcService
    {
        private readonly ILogger<DiaryGrpcService> _logger;
        private readonly IDiaryFileService _diaryFileService;

        public DiaryGrpcService(ILogger<DiaryGrpcService> logger, IDiaryFileService diaryFileService)
        {
            _logger = logger;
            _diaryFileService = diaryFileService;
        }

        public async Task<DiaryGrpcResponse> GetDiaryAsync(DiaryGrpcRequest request, CallContext context = default)
        {
            var response = new DiaryGrpcResponse();
            try
            {
                var diary = await _diaryFileService.GetDiaryAsync(request.Date);
                if (diary == null)
                {
                    response.NotFound = true;
                }
                else
                {
                    var diaryRuntime = new DiaryRuntime(diary);
                    if (request.ExtractRuntime)
                    {
                        diaryRuntime.ExtractRuntimeData();
                    }

                    List<Diary> allDiaries = null;
                    if (request.ExtractPrev)
                    {
                        allDiaries = await _diaryFileService.GetDiariesAsync();
                        var prevDiary = allDiaries.FirstOrDefault(x => x.Date < request.Date);
                        if (prevDiary != null)
                        {
                            diaryRuntime.Prev = new DiaryRuntime(prevDiary);
                        }
                    }

                    if (request.ExtractNext)
                    {
                        allDiaries ??= await _diaryFileService.GetDiariesAsync();
                        var nextDiary = allDiaries.LastOrDefault(x => x.Date > request.Date);
                        if (nextDiary != null)
                        {
                            diaryRuntime.Next = new DiaryRuntime(nextDiary);
                        }
                    }

                    response.DiaryRuntime = diaryRuntime;
                }
            }
            catch (Exception ex)
            {
                response.IsOk = false;
                response.Message = ex.Message;
                _logger.LogError(ex, $"Get diary failed: {JsonUtil.Serialize(request)}");
            }

            return response;
        }

        public async Task<DiaryGrpcResponse> GetDiariesAsync(DiaryGrpcRequest request, CallContext context = default)
        {
            var response = new DiaryGrpcResponse();
            try
            {
                var diaries = await _diaryFileService.GetDiariesAsync(request.Offset, request.Count, request.Year);
                var diaryRuntimeList = new List<DiaryRuntime>();
                foreach (var diary in diaries)
                {
                    var diaryRuntime = new DiaryRuntime(diary);
                    if (request.ExtractRuntime)
                    {
                        diaryRuntime.ExtractRuntimeData();
                    }
                    diaryRuntimeList.Add(diaryRuntime);
                }

                response.DiaryRuntimeList = diaryRuntimeList;
            }
            catch (Exception ex)
            {
                response.IsOk = false;
                response.Message = ex.Message;
                _logger.LogError(ex, $"Get diaries failed: {JsonUtil.Serialize(request)}");
            }

            return response;
        }

        public async Task<DiaryGrpcResponse> GetDiariesCountAsync(DiaryGrpcRequest request, CallContext context = default)
        {
            var response = new DiaryGrpcResponse();
            try
            {
                response.Count = await _diaryFileService.GetDiaryCountAsync();
            }
            catch (Exception ex)
            {
                response.IsOk = false;
                response.Message = ex.Message;
                _logger.LogError(ex, $"Get diaries count failed: {JsonUtil.Serialize(request)}");
            }

            return response;
        }

        public async Task<DiaryGrpcResponse> AddDiaryAsync(DiaryGrpcRequest request, CallContext context = default)
        {
            var response = new DiaryGrpcResponse();
            try
            {
                await _diaryFileService.AddDiaryAsync(request.Diary);
                response.Diary = request.Diary;
            }
            catch (Exception ex)
            {
                response.IsOk = false;
                response.Message = ex.Message;
                _logger.LogError(ex, $"Get diaries failed: {JsonUtil.Serialize(request)}");
            }

            return response;
        }

        public async Task<DiaryGrpcResponse> UpdateDiaryAsync(DiaryGrpcRequest request, CallContext context = default)
        {
            var response = new DiaryGrpcResponse();
            try
            {
                await _diaryFileService.UpdateDiaryAsync(request.Diary);
                response.Diary = request.Diary;
            }
            catch (Exception ex)
            {
                response.IsOk = false;
                response.Message = ex.Message;
                _logger.LogError(ex, $"Get diaries failed: {JsonUtil.Serialize(request)}");
            }

            return response;
        }
    }
}
