using System.Collections.Generic;
using System.Runtime.Serialization;
using Laobian.Share.Logger;

namespace Laobian.Share.Grpc.Request
{
    [DataContract]
    public class LogRequest
    {
        [DataMember(Order = 1)]
        public string Logger { get; set; }

        [DataMember(Order = 2)]
        public int Days { get; set; }

        [DataMember(Order = 3)]
        public int MinLevel { get; set; }

        [DataMember(Order = 4)]
        public List<LaobianLog> Logs { get; set; }
    }
}
