using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Share
{
    public class LaobianConfigException : Exception
    {
        public LaobianConfigException(string configName):base($"Invalid configuration: {configName}"){}
    }
}
