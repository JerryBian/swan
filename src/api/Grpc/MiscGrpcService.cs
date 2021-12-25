using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using Laobian.Share.Grpc.Service;
using ProtoBuf.Grpc;

namespace Laobian.Api.Grpc
{
    public class MiscGrpcService : IMiscGrpcService
    {
        public async Task<MiscGrpcResponse> ShutdownSiteAsync(MiscGrpcRequest request, CallContext context = default)
        {
            throw new System.NotImplementedException();
        }

        public async Task<MiscGrpcResponse> GetSiteStatAsync(MiscGrpcRequest request, CallContext context = default)
        {
            throw new System.NotImplementedException();
        }

        public async Task<MiscGrpcResponse> GetDbStatAsync(MiscGrpcRequest request, CallContext context = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
