using System.ServiceModel;
using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using ProtoBuf.Grpc;

namespace Laobian.Share.Grpc.Service;

[ServiceContract]
public interface INoteGrpcService
{
    [OperationContract]
    Task<NoteGrpcResponse> GetNoteStatCountPerYearAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> GetNoteStatWordsPerYearAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> GetNotesAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> GetNoteAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> AddNoteAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> UpdateNoteAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> DeleteNoteAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> GetNoteTagStatCountPerYearAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> GetNoteTagsAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> GetNoteTagByIdAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> GetNoteTagByLinkAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> AddNoteTagAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> UpdateNoteTagAsync(NoteGrpcRequest request, CallContext context = default);

    [OperationContract]
    Task<NoteGrpcResponse> DeleteNoteTagAsync(NoteGrpcRequest request, CallContext context = default);
}