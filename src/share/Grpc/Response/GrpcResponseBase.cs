using System.ComponentModel;
using System.Runtime.Serialization;
using Laobian.Share.Grpc.Request;
using ProtoBuf;

namespace Laobian.Share.Grpc.Response;

[DataContract]
[ProtoInclude(100, typeof(LogGrpcResponse))]
[ProtoInclude(200, typeof(BlogGrpcResponse))]
[ProtoInclude(300, typeof(FileGrpcResponse))]
[ProtoInclude(400, typeof(DiaryGrpcResponse))]
[ProtoInclude(500, typeof(NoteGrpcResponse))]
public class GrpcResponseBase
{
    [DataMember(Order = 1)]
    [DefaultValue(true)]
    public bool IsOk { get; set; } = true;

    [DataMember(Order = 2)] public string Message { get; set; }
}