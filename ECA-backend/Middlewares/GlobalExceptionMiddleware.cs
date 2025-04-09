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
                // Pass the request to the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

                // Handle the exception
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Set default values
            var code = HttpStatusCode.InternalServerError; // 500
            string result = string.Empty;
            string message = "An unexpected error occurred";

            // Customize response based on exception type (if needed)
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

                // Add more specific exception types as needed
                default:
                    break;
            }

            // Create error response
            var errorResponse = new
            {
                StatusCode = (int)code,
                Message = message,
                Detailed = exception.Message  // Remove in production if sensitive
            };

            // Serialize to JSON
            result = JsonSerializer.Serialize(errorResponse);

            // Set response details
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            // Write response
            return context.Response.WriteAsync(result);
        }
    }

    // Extension method to easily add the middleware to the pipeline
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
