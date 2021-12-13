using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Logger;

namespace Laobian.Share.Grpc.Response
{
    [DataContract]
    public class LogResponse : ResponseBase
    {
        [DataMember(Order = 1)]
        public List<LaobianLog> Logs { get; set; }
    }
}
