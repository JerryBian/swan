using System.Collections.Generic;
using System.Runtime.Serialization;
using Laobian.Share.Site.Read;

namespace Laobian.Share.Grpc.Response;

[DataContract]
public class ReadGrpcResponse : GrpcResponseBase
{
    [DataMember(Order = 1)] public List<ReadItemRuntime> ReadItems { get; set; }

    [DataMember(Order = 2)] public ReadItem ReadItem { get; set; }

    [DataMember(Order = 3)] public ReadItemRuntime ReadItemRuntime { get; set; }
}