using IGrok.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace IGrok.Handlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
                                                Exception exception,
                                                CancellationToken cancellationToken)
    {
        if (exception is IGrokException igrokEx)
        {
            logger.LogWarning(exception, "A handled business exception occurred: {Message}", exception.Message);
        }
        else
        {
            logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
        }

        var (statusCode, errorCode, message) = GetResponseDetails(exception);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = errorCode,
            Detail = message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (int StatusCode, string ErrorCode, string Message) GetResponseDetails(Exception exception)
    {
        return exception switch
        {
            IGrokException igrokEx => ((int)igrokEx.StatusCode, igrokEx.ErrorCode, igrokEx.Message),
            _ => ((int)HttpStatusCode.InternalServerError, "InternalServerError", "An unexpected internal server error has occurred.")
        };
    }
}
