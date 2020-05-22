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

            return $"{context.Connection.RemoteIpAddress}\t{context.Request.GetDisplayUrl()}\t{message}";
        }
    }
}
