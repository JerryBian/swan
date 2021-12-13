using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using ProtoBuf.Grpc;

namespace Laobian.Share.Grpc.Service
{
    [ServiceContract]
    public interface ILogService
    {
        [OperationContract]
        Task<LogResponse> AddLogsAsync(LogRequest request, CallContext context = default);

        [OperationContract]
        Task<LogResponse> GetLogsAsync(LogRequest request, CallContext context = default);
    }
}
