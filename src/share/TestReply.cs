using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Share
{
    [DataContract]
    public class TestReply
    {
        [DataMember(Order = 1)]
        public DateTime Now { get; set; }
    }
}
