using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Laobian.Share.Grpc.Response;

[DataContract]
public class MiscGrpcResponse : GrpcResponseBase
{
    [DataMember(Order = 1)] public List<GitFileStat> DbStats { get; set; }

    [DataMember(Order = 2)] public SiteStat SiteStat { get; set; }
}