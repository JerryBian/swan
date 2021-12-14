using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Site.Read;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace Laobian.Api.Grpc
{
    public class ReadGrpcService : IReadGrpcService
    {
        private readonly IFileRepository _fileRepository;
        private readonly ILogger<ReadGrpcService> _logger;

        public ReadGrpcService(IFileRepository fileRepository, ILogger<ReadGrpcService> logger)
        {
            _logger = logger;
            _fileRepository = fileRepository;
        }

        private ReadItemRuntime GetReadItemRuntime(ReadItem readItem, bool extractRuntime)
        {
            var runtime = new ReadItemRuntime(readItem);
            if (extractRuntime)
            {
                runtime.ExtractRuntimeData();
            }

            return runtime;
        }

        public async Task<ReadGrpcResponse> GetReadItemsAsync(ReadGrpcRequest request, CallContext context = default)
        {
            var response = new ReadGrpcResponse();
            try
            {
                var readItemRuntime = new List<ReadItemRuntime>();
                var readItems = await _fileRepository.GetReadItemsAsync();
                foreach (var readItem in readItems)
                {
                    readItemRuntime.Add(GetReadItemRuntime(readItem, request.ExtractRuntime));
                }

                response.ReadItems = readItemRuntime;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get read items failed. {JsonUtil.Serialize(request)}");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ReadGrpcResponse> GetReadItemAsync(ReadGrpcRequest request, CallContext context = default)
        {
            var response = new ReadGrpcResponse();
            try
            {
                var readItems = await _fileRepository.GetReadItemsAsync();
                var readItem = readItems.FirstOrDefault(x => x.Id == request.ReadItemId);
                response.ReadItemRuntime = GetReadItemRuntime(readItem, request.ExtractRuntime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get read item failed. {JsonUtil.Serialize(request)}");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ReadGrpcResponse> AddReadItemAsync(ReadGrpcRequest request, CallContext context = default)
        {
            var response = new ReadGrpcResponse();
            try
            {
                await _fileRepository.AddReadItemAsync(request.ReadItem);
                response.ReadItem = request.ReadItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Add read item failed. {JsonUtil.Serialize(request)}");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ReadGrpcResponse> UpdateReadItemAsync(ReadGrpcRequest request, CallContext context = default)
        {
            var response = new ReadGrpcResponse();
            try
            {
                await _fileRepository.UpdateReadItemAsync(request.ReadItem);
                response.ReadItem = request.ReadItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Update read item failed. {JsonUtil.Serialize(request)}");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ReadGrpcResponse> DeleteReadItemAsync(ReadGrpcRequest request, CallContext context = default)
        {
            var response = new ReadGrpcResponse();
            try
            {
                await _fileRepository.DeleteReadItemAsync(request.ReadItemId);
                response.ReadItem = request.ReadItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete read item failed. {JsonUtil.Serialize(request)}");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
