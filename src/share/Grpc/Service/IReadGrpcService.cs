﻿using System.ServiceModel;
using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using ProtoBuf.Grpc;

namespace Laobian.Share.Grpc.Service;

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