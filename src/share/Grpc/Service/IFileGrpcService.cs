using System.ServiceModel;
using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using ProtoBuf.Grpc;

namespace Laobian.Share.Grpc.Service;

[ServiceContract]
public interface IFileGrpcService
{
    [OperationContract]
    Task<FileGrpcResponse> AddFileAsync(FileGrpcRequest request, CallContext context = default);
}