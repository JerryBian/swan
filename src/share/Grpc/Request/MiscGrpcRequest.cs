using System.Runtime.Serialization;
using Laobian.Share.Site;

namespace Laobian.Share.Grpc.Request;

[DataContract]
public class MiscGrpcRequest
{
    [DataMember(Order = 1)] public LaobianSite Site { get; set; }

    [DataMember(Order = 2)] public string Message { get; set; }
}