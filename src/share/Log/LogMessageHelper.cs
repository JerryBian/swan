using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Laobian.Share.Log
{
    public class LogMessageHelper
    {
        public static string Format(string message, HttpContext context = null)
        {
            if (context == null)
            {
                return message;
            }

            return $"{message}{Environment.NewLine}{context.Connection.RemoteIpAddress}{Environment.NewLine}{context.Request.GetDisplayUrl()}{Environment.NewLine}{context.Request.Headers["User-Agent"]}";
        }
    }
}
