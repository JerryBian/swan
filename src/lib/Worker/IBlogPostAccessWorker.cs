using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Lib.Worker
{
    public interface IBlogPostAccessWorker
    {
        void Add(string id);

        Task ProcessAsync();

        Task StopAsync();
    }
}
