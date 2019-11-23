using System;

namespace Laobian.Share.Config
{
    public class AppConfigException : Exception
    {
        public AppConfigException(string message) : base(message)
        {
        }
    }
}