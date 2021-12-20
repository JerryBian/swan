using System.Runtime.Serialization;

namespace Laobian.Share.Grpc.Response
{
    [DataContract]
    public class FileGrpcResponse : GrpcResponseBase
    {
        [DataMember(Order = 1)]
        public string Url { get; set; }
    }
}
