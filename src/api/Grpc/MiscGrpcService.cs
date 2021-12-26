using System;
using System.Net.Http;
using System.Threading.Tasks;
using Laobian.Api.HttpClients;
using Laobian.Share;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Site;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace Laobian.Api.Grpc
{
    public class MiscGrpcService : IMiscGrpcService
    {
        
        private readonly BlogSiteHttpClient _blogHttpClient;
        private readonly JarvisSiteHttpClient _jarvisHttpClient;
        private readonly ILogger<MiscGrpcService> _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public MiscGrpcService(ILogger<MiscGrpcService> logger, IHostApplicationLifetime appLifetime, BlogSiteHttpClient blogHttpClient, JarvisSiteHttpClient jarvisHttpClient)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _blogHttpClient = blogHttpClient;
            _jarvisHttpClient = jarvisHttpClient;
        }

        public async Task<MiscGrpcResponse> ReloadBlogCacheAsync(MiscGrpcRequest request, CallContext context = default)
        {
            var response = new MiscGrpcResponse();
            try
            {
                await _blogHttpClient.ReloadBlogDataAsync();
            }
            catch (Exception ex)
            {
                response.IsOk = false;
                response.Message = ex.Message;
                _logger.LogError(ex, $"{nameof(ReloadBlogCacheAsync)} failed.");
            }

            return response;
        }

        public async Task<MiscGrpcResponse> ShutdownSiteAsync(MiscGrpcRequest request, CallContext context = default)
        {
            var response = new MiscGrpcResponse();
            try
            {
                if (request.Site == LaobianSite.Api)
                {
                    _appLifetime.StopApplication();
                }else if (request.Site == LaobianSite.Blog)
                {
                    await _blogHttpClient.ShutdownAsync();
                }else if (request.Site == LaobianSite.Jarvis)
                {
                    await _jarvisHttpClient.ShutdownAsync();
                }
                else
                {
                    response.IsOk = false;
                    response.Message = $"Shutdown site {request.Site} not supported.";
                }
            }
            catch (Exception ex)
            {
                response.IsOk = false;
                response.Message = ex.Message;
                _logger.LogError(ex, $"{nameof(ShutdownSiteAsync)} failed.");
            }

            return response;
        }

        public async Task<MiscGrpcResponse> GetSiteStatAsync(MiscGrpcRequest request, CallContext context = default)
        {
            var response = new MiscGrpcResponse();
            try
            {
                if (request.Site == LaobianSite.Jarvis)
                {
                    response.SiteStat = await _jarvisHttpClient.GetSiteStatAsync();
                }
                else if (request.Site == LaobianSite.Blog)
                {
                    response.SiteStat = await _blogHttpClient.GetSiteStatAsync();
                }
                else if (request.Site == LaobianSite.Api)
                {
                    response.SiteStat = SiteStatHelper.Get();
                }
                else
                {
                    response.IsOk = false;
                    response.Message = $"Getting site {request.Site} stat not supported.";
                }
            }
            catch (Exception ex)
            {
                response.IsOk = false;
                response.Message = ex.Message;
                _logger.LogError(ex, $"{nameof(GetSiteStatAsync)} failed.");
            }

            return response;
        }

        public async Task<MiscGrpcResponse> GetDbStatAsync(MiscGrpcRequest request, CallContext context = default)
        {
            var response = new MiscGrpcResponse();
            try
            {

            }
            catch (Exception ex)
            {
                response.IsOk = false;
                response.Message = ex.Message;
                _logger.LogError(ex, $"{nameof(GetDbStatAsync)} failed.");
            }

            return response;
        }
    }
}
