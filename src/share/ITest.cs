using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf.Grpc;

namespace Laobian.Share
{
    [ServiceContract]
    public interface ITest
    {
        [OperationContract]
        Task<TestReply> GetNow(TestRequest r, CallContext context = default);
    }
}
