using System.Net;

namespace EcommerceAPI.Middlewares
{
    /// <summary>
    /// Middleware to handle exceptions globally in the application.
    /// Logs the exception and returns a standardized JSON response.
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger used to log exceptions.</param>
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Processes an HTTP request and handles any exceptions that occur.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles the specified exception by logging it and writing a JSON response to the client.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="ex">The exception to handle.</param>
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = GetStatusCode(ex);

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = ex.Message,
                Detail = ex.InnerException?.Message
            };

            await context.Response.WriteAsJsonAsync(response);
        }

        /// <summary>
        /// Maps known exception types to corresponding HTTP status codes.
        /// </summary>
        /// <param name="ex">The exception to evaluate.</param>
        /// <returns>The appropriate HTTP status code.</returns>
        private static int GetStatusCode(Exception ex)
        {
            return ex switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                InvalidOperationException => (int)HttpStatusCode.Forbidden,
                _ => (int)HttpStatusCode.InternalServerError
            };
        }
    }

}
