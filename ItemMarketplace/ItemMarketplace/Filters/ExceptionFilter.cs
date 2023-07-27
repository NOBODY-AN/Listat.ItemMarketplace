using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ItemMarketplace.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger _logger;
        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {

            _logger = logger;

        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = 500;
            context.Result = new JsonResult("SYSTEM_EXCEPTION");
            _logger.LogError("System exception message: {exception}, inner exception: {@innerException}", context.Exception.Message, context.Exception.InnerException);

            context.ExceptionHandled = true;
        }
    }
}
