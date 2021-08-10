using System;

namespace Laobian.Share.Option
{
    public class LaobianOptionException : Exception
    {
        public LaobianOptionException(string configName) : base($"Invalid configuration: {configName}")
        {
        }
    }
}