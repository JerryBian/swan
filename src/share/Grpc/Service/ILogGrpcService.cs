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
    Task<LogResponse> AddLogsAsync(LogRequest request, CallContext context = default);

    [OperationContract]
    Task<LogResponse> GetLogsAsync(LogRequest request, CallContext context = default);
}