using System;
using System.Collections.Generic;
using System.Text;

namespace Laobian.Share.Config
{
    public class AppConfigException : Exception
    {
        public AppConfigException(string message) : base(message) { }
    }
}
