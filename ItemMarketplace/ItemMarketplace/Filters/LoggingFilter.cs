using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ItemMarketplace.Filters
{
    public class LoggingFilter : IAsyncActionFilter
    {
        private readonly ILogger _logger;

        public LoggingFilter(ILogger<LoggingFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ActionExecutedContext actionExecutedContext = await next();

            ControllerActionDescriptor? actionController = actionExecutedContext.ActionDescriptor as ControllerActionDescriptor;

            object? responseObject = (actionExecutedContext.Result as ObjectResult)?.Value;
            object? requestBody = null;
            try
            {
                requestBody = GetRequestBody(context);
            }
            catch
            {

            }


            string logMessage = "{controllerName} - {method} request with route {path} request: {@requestBody}, response: {@responseObject}, status code: {statusCode}";
            
            LogLevel level = actionExecutedContext != null && actionExecutedContext.HttpContext.Response.StatusCode == 200
                ? LogLevel.Information
                : LogLevel.Warning;

            _logger.Log(level, logMessage,
                    actionController?.ControllerName ?? "Unknown controller name",
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path,
                    requestBody,
                    responseObject,
                    actionExecutedContext?.HttpContext.Response.StatusCode);
        }

        private static object? GetRequestBody(ActionExecutingContext context)
        {
            string? bodyParabeterKey = context.ActionDescriptor.Parameters.FirstOrDefault(x => x.BindingInfo?.BindingSource == BindingSource.Body)?.Name;

            if (string.IsNullOrEmpty(bodyParabeterKey))
            {
                return null;
            }

            return context.ActionArguments[bodyParabeterKey];
        }
    }
}
