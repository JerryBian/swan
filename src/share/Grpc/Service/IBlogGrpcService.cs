using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using Laobian.Share.Site.Blog;
using ProtoBuf.Grpc;

namespace Laobian.Share.Grpc.Service
{
    [ServiceContract]
    public interface IBlogGrpcService
    {
        [OperationContract]
        Task<BlogResponse> GetPostsAsync(BlogRequest request, CallContext context = default);

        [OperationContract]
        Task<BlogResponse> GetPostAsync(BlogRequest request, CallContext context = default);

        [OperationContract]
        Task<BlogResponse> AddPostAsync(BlogRequest request, CallContext context = default);

        [OperationContract]
        Task<BlogResponse> UpdatePostAsync(BlogRequest request, CallContext context = default);

        [OperationContract]
        Task<BlogResponse> AddPostAccessAsync(BlogRequest request, CallContext context = default);

        [OperationContract]
        Task<BlogResponse> GetTagsAsync(CallContext context = default);

        [OperationContract]
        Task<BlogResponse> GetTagAsync(BlogRequest request, CallContext context = default);

        [OperationContract]
        Task<BlogResponse> AddTagAsync(BlogRequest request, CallContext context = default);

        [OperationContract]
        Task<BlogResponse> UpdateTagAsync(BlogRequest request, CallContext context = default);

        [OperationContract]
        Task<BlogResponse> DeleteTagAsync(BlogRequest request, CallContext context = default);
    }
}
