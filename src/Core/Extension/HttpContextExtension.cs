namespace Swan.Core.Extension
{
    public static class HttpContextExtension
    {
        public static bool IsAuthorized(this HttpContext context)
        {
            return context?.User?.Identity?.IsAuthenticated == true;
        }

        public static string GetIpAddress(this HttpContext context)
        {
            return context?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }
    }
}
