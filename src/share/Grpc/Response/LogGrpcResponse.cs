using System.Collections.Generic;
using System.Runtime.Serialization;
using Laobian.Share.Logger;

namespace Laobian.Share.Grpc.Response;

[DataContract]
public class LogGrpcResponse : GrpcResponseBase
{
    [DataMember(Order = 1)] public List<LaobianLog> Logs { get; set; }
}