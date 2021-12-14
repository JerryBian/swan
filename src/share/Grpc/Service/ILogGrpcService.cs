using System.ServiceModel;
using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using ProtoBuf.Grpc;

namespace Laobian.Share.Grpc.Service;

[ServiceContract]
public interface ILogGrpcService
{
    [OperationContract]
    Task<LogGrpcResponse> AddLogsAsync(LogGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<LogGrpcResponse> GetLogsAsync(LogGrpcRequest request, CallContext context = default);
}