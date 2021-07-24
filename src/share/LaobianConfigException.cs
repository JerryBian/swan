using System;

namespace Laobian.Share
{
    public class LaobianConfigException : Exception
    {
        public LaobianConfigException(string configName) : base($"Invalid configuration: {configName}")
        {
        }
    }
}