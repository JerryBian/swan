using System;

namespace Laobian.Share.Config
{
    public class LaobianConfigException : Exception
    {
        public LaobianConfigException(string configName) : base($"Invalid configuration: {configName}")
        {
        }
    }
}