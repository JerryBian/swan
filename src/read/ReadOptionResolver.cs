using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Option;
using Microsoft.Extensions.Configuration;

namespace Laobian.Read
{
    public class ReadOptionResolver : CommonOptionResolver
    {
        public void Resolve(ReadOption option, IConfiguration configuration)
        {
            base.Resolve(option, configuration);
        }
    }
}
