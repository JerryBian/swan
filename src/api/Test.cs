using Laobian.Share;
using ProtoBuf.Grpc;
using System;
using System.Threading.Tasks;

namespace Laobian.Api
{
    public class Test : ITest
    {
        public async Task<TestReply> GetNow(TestRequest r, CallContext context = default)
        {
            return await Task.FromResult(new TestReply{Now = DateTime.Now});
        }
    }
}
