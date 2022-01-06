using System.ServiceModel;
using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using ProtoBuf.Grpc;

namespace Laobian.Share.Grpc.Service;

[ServiceContract]
public interface IDiaryGrpcService
{
    [OperationContract]
    Task<DiaryGrpcResponse> GetDiaryAsync(DiaryGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<DiaryGrpcResponse> GetDiariesAsync(DiaryGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<DiaryGrpcResponse> GetDiaryDatesAsync(DiaryGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<DiaryGrpcResponse> AddDiaryAsync(DiaryGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<DiaryGrpcResponse> UpdateDiaryAsync(DiaryGrpcRequest request, CallContext context = default);
}