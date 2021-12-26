using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Share.Grpc.Response
{
    [DataContract]
    public class MiscGrpcResponse : GrpcResponseBase
    {
        [DataMember(Order = 1)]
        public List<GitFileStat> DbStats { get; set; }

        [DataMember(Order = 2)]
        public SiteStat SiteStat { get; set; }
    }
}
