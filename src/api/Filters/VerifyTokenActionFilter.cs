using System.Threading.Tasks;
using Laobian.Share;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Laobian.Api.Filter
{
    public class VerifyTokenActionFilter : IAsyncActionFilter
    {
        private readonly string _token;

        public VerifyTokenActionFilter(string token)
        {
            _token = token;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.ContainsKey(Constants.ApiRequestHeaderToken))
            {
                context.Result = new BadRequestObjectResult("No API token set.");
                return;
            }

            if (_token != context.HttpContext.Request.Headers[Constants.ApiRequestHeaderToken])
            {
                context.Result = new BadRequestObjectResult($"Invalid API token set: {context.HttpContext.Request.Headers[Constants.ApiRequestHeaderToken]}");
                return;
            }

            await next();
        }
    }
}