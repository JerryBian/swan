using System.ServiceModel;
using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using ProtoBuf.Grpc;

namespace Laobian.Share.Grpc.Service;

[ServiceContract]
public interface IMiscGrpcService
{
    [OperationContract]
    Task<MiscGrpcResponse> ShutdownSiteAsync(MiscGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<MiscGrpcResponse> GetSiteStatAsync(MiscGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<MiscGrpcResponse> GetDbStatAsync(MiscGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<MiscGrpcResponse> PersistentGitFileAsync(MiscGrpcRequest request, CallContext context = default);
}