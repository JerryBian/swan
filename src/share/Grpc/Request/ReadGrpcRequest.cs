using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Site.Read;

namespace Laobian.Share.Grpc.Request
{
    [DataContract]
    public class ReadGrpcRequest
    {
        [DataMember(Order = 1)]
        public string ReadItemId { get; set; }

        [DataMember(Order = 2)]
        public ReadItem ReadItem { get; set; }

        [DataMember(Order = 3)]
        public bool ExtractRuntime { get; set; }
    }
}
