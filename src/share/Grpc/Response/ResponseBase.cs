using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using ProtoBuf;

namespace Laobian.Share.Grpc.Response
{
    [DataContract]
    [ProtoInclude(100, typeof(LogResponse))]
    public class ResponseBase
    {
        [DataMember(Order = 1)] public bool IsOk { get; set; } = true;

        [DataMember(Order = 2)]
        public string Message { get; set; }
    }
}
