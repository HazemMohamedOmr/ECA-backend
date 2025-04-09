using System.Net;
using System.Text.Json;

namespace ECA_backend.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            string result = string.Empty;
            string message = "An unexpected error occurred";

            switch (exception)
            {
                case ArgumentException argEx:
                    code = HttpStatusCode.BadRequest; // 400
                    message = argEx.Message;
                    break;

                case UnauthorizedAccessException:
                    code = HttpStatusCode.Unauthorized; // 401
                    message = "Unauthorized access";
                    break;

                case KeyNotFoundException:
                    code = HttpStatusCode.NotFound; // 404
                    message = "Resource not found";
                    break;

                default:
                    break;
            }

            var errorResponse = new
            {
                StatusCode = (int)code,
                Message = message,
                Detailed = exception.Message
            };

            result = JsonSerializer.Serialize(errorResponse);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}
