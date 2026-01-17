using System.Net;
using System.Text.Json;

namespace CustomerManagementSystem.Utility
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                
                if (IsAjaxRequest(context.Request))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        success = false,
                        message = "Something went wrong. Please try again later."
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
                else
                {
                    
                    context.Response.Redirect("/ExceptionHandle/Error");
                }
            }
        }

        private static bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest"
                   || request.Headers["Accept"].ToString().Contains("application/json");
        }
    }
}