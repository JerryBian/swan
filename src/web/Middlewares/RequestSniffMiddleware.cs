using Swan.Core.Extension;
using Swan.Core.Service;
using Swan.Core.Store;

namespace Swan.Web.Middlewares
{
    public class RequestSniffMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISwanStore _swanStore;
        private readonly ISwanService _swanService;

        public RequestSniffMiddleware(RequestDelegate next, ISwanStore swanStore, ISwanService swanService)
        {
            _next = next;
            _swanStore = swanStore;
            _swanService = swanService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if(context.Response.StatusCode == 200)
            {
                await _swanStore.AddPageHitAsync(context.Request.Path.Value, context.GetIpAddress());
            }
        }
    }
}
