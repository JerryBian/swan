using System.Runtime.Serialization;

namespace Laobian.Share.Grpc.Request;

[DataContract]
public class FileGrpcRequest
{
    [DataMember(Order = 1)] public string FileName { get; set; }

    [DataMember(Order = 2)] public byte[] Content { get; set; }
}