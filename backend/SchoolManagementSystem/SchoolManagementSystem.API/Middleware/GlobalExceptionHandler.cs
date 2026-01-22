using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.API.Exceptions;
using System.Text.Json;

namespace SchoolManagementSystem.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Handle request cancellation separately (common in real-world scenarios)
            if (exception is OperationCanceledException)
            {
                _logger.LogInformation(
                    "Request was cancelled: {Method} {Path}",
                    httpContext.Request.Method,
                    httpContext.Request.Path);

                httpContext.Response.StatusCode = 499; // Non-standard but widely recognized
                return true;
            }

            // Log based on exception severity
            var (statusCode, title, logLevel) = MapException(exception);

            LogException(exception, httpContext, logLevel);

            // Build problem details response
            var problemDetails = CreateProblemDetails(
                httpContext,
                exception,
                statusCode,
                title);

            // Write response
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                },
                cancellationToken);

            return true;
        }

        private (int StatusCode, string Title, LogLevel LogLevel) MapException(Exception exception)
        {
            return exception switch
            {
                // Application-specific exceptions
                NotFoundException => (
                    StatusCodes.Status404NotFound,
                    "Resource Not Found",
                    LogLevel.Warning),

                DuplicateClassException => (
                    StatusCodes.Status409Conflict,
                    "Duplicate Resource",
                    LogLevel.Warning),

                BusinessRuleViolationException => (
                    StatusCodes.Status422UnprocessableEntity, // More semantic than 400
                    "Business Rule Violation",
                    LogLevel.Warning),

                // Built-in .NET exceptions
                KeyNotFoundException => (
                    StatusCodes.Status404NotFound,
                    "Resource Not Found",
                    LogLevel.Warning),

                ArgumentException or ArgumentNullException => (
                    StatusCodes.Status400BadRequest,
                    "Invalid Request",
                    LogLevel.Warning),

                InvalidOperationException => (
                    StatusCodes.Status400BadRequest,
                    "Invalid Operation",
                    LogLevel.Warning),

                UnauthorizedAccessException => (
                    StatusCodes.Status403Forbidden,
                    "Access Denied",
                    LogLevel.Warning),

                // Database exceptions (if using EF Core)
                Microsoft.EntityFrameworkCore.DbUpdateException => (
                    StatusCodes.Status409Conflict,
                    "Database Conflict",
                    LogLevel.Error),

                // Default: Internal Server Error
                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    LogLevel.Error)
            };
        }

        private ProblemDetails CreateProblemDetails(
            HttpContext context,
            Exception exception,
            int statusCode,
            string title)
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = GetProblemTypeUri(statusCode),
                Instance = context.Request.Path,
            };

            // Add user-friendly detail message
            problemDetails.Detail = GetDetailMessage(exception, statusCode);

            // Add trace ID for correlation
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;

            // Add timestamp
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

            // In development, include additional debugging information
            if (_environment.IsDevelopment())
            {
                problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;

                // Include inner exception details
                if (exception.InnerException != null)
                {
                    problemDetails.Extensions["innerException"] = new
                    {
                        type = exception.InnerException.GetType().Name,
                        message = exception.InnerException.Message
                    };
                }
            }

            return problemDetails;
        }

        private string GetDetailMessage(Exception exception, int statusCode)
        {

            if (statusCode >= 400 && statusCode < 500)
            {
                return exception.Message;
            }

            // For server errors (5xx), return generic message in production
            if (_environment.IsProduction())
            {
                return "An unexpected error occurred. Please contact support if the problem persists.";
            }

            // In non-production, include the actual message for debugging
            return exception.Message;
        }

        private static string GetProblemTypeUri(int statusCode)
        {
            // Return RFC 7807 compliant type URIs
            return statusCode switch
            {
                400 => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                401 => "https://tools.ietf.org/html/rfc7235#section-3.1",
                403 => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                404 => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                409 => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                422 => "https://tools.ietf.org/html/rfc4918#section-11.2",
                500 => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                _ => $"https://httpstatuses.com/{statusCode}"
            };
        }

        private void LogException(
            Exception exception,
            HttpContext context,
            LogLevel logLevel)
        {
            var requestInfo = new
            {
                Method = context.Request.Method,
                Path = context.Request.Path.Value,
                QueryString = context.Request.QueryString.Value,
                TraceId = context.TraceIdentifier,
                UserAgent = context.Request.Headers.UserAgent.ToString(),
                RemoteIp = context.Connection.RemoteIpAddress?.ToString()
            };

          
            _logger.Log(
                logLevel,
                exception,
                "Exception occurred during request processing. " +
                "Method: {Method}, Path: {Path}, TraceId: {TraceId}, Exception: {ExceptionType}",
                requestInfo.Method,
                requestInfo.Path,
                requestInfo.TraceId,
                exception.GetType().Name);

   
            if (logLevel == LogLevel.Error || logLevel == LogLevel.Critical)
            {
                _logger.LogError(
                    "Full request details: {@RequestInfo}",
                    requestInfo);
            }
        }
    }
}