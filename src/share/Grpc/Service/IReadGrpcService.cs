using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using Laobian.Share.Site.Read;
using ProtoBuf.Grpc;

namespace Laobian.Share.Grpc.Service
{
    [ServiceContract]
    public interface IReadGrpcService
    {
        [OperationContract]
        Task<ReadGrpcResponse> GetReadItemsAsync(ReadGrpcRequest request, CallContext context = default);

        [OperationContract]
        Task<ReadGrpcResponse> GetReadItemAsync(ReadGrpcRequest request, CallContext context = default);

        [OperationContract]
        Task<ReadGrpcResponse> AddReadItemAsync(ReadGrpcRequest request, CallContext context = default);

        [OperationContract]
        Task<ReadGrpcResponse> UpdateReadItemAsync(ReadGrpcRequest request, CallContext context = default);

        [OperationContract]
        Task<ReadGrpcResponse> DeleteReadItemAsync(ReadGrpcRequest request, CallContext context = default);
    }
}
