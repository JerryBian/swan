using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Site.Jarvis;
using ProtoBuf.Grpc;

namespace Laobian.Share.Grpc.Service
{
    [ServiceContract]
    public interface IDiaryGrpcService
    {
        [OperationContract]
        Task<DiaryGrpcResponse> GetDiaryAsync(DiaryGrpcRequest request, CallContext context = default);

        [OperationContract]
        Task<DiaryGrpcResponse> GetDiariesAsync(DiaryGrpcRequest request, CallContext context = default);

        [OperationContract]
        Task<DiaryGrpcResponse> GetDiariesCountAsync(DiaryGrpcRequest request, CallContext context = default);

        [OperationContract]
        Task<DiaryGrpcResponse> AddDiaryAsync(DiaryGrpcRequest request, CallContext context = default);

        [OperationContract]
        Task<DiaryGrpcResponse> UpdateDiaryAsync(DiaryGrpcRequest request, CallContext context = default);
    }
}
