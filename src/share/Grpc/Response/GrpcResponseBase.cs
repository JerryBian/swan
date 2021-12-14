using System.Runtime.Serialization;
using ProtoBuf;

namespace Laobian.Share.Grpc.Response;

[DataContract]
[ProtoInclude(100, typeof(LogGrpcResponse))]
[ProtoInclude(200, typeof(BlogGrpcResponse))]
public class GrpcResponseBase
{
    [DataMember(Order = 1)] public bool IsOk { get; set; } = true;

    [DataMember(Order = 2)] public string Message { get; set; }
}