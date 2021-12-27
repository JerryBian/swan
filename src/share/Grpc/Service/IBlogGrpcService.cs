using System.ServiceModel;
using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using ProtoBuf.Grpc;

namespace Laobian.Share.Grpc.Service;

[ServiceContract]
public interface IBlogGrpcService
{
    [OperationContract]
    Task<MiscGrpcResponse> ReloadBlogCacheAsync(BlogGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<BlogGrpcResponse> GetPostsAsync(BlogGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<BlogGrpcResponse> GetPostAsync(BlogGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<BlogGrpcResponse> AddPostAsync(BlogGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<BlogGrpcResponse> UpdatePostAsync(BlogGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<BlogGrpcResponse> AddPostAccessAsync(BlogGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<BlogGrpcResponse> GetTagsAsync(CallContext context = default);

    [OperationContract]
    Task<BlogGrpcResponse> GetTagAsync(BlogGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<BlogGrpcResponse> AddTagAsync(BlogGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<BlogGrpcResponse> UpdateTagAsync(BlogGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<BlogGrpcResponse> DeleteTagAsync(BlogGrpcRequest request, CallContext context = default);
}