using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception");
        var (status, title) = exception switch
        {
            AppException appEx => (appEx.StatusCode, GetTitle(appEx.StatusCode)),
            _ => (StatusCodes.Status500InternalServerError, "Server error")
        };
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception is AppException
                ? exception.Message
                : "An unexpected error occurred."
        };
        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static string GetTitle(int status) => status switch
    {
        404 => "Not Found",
        409 => "Conflict",
        400 => "Bad Request",
        502 => "Bad Gateway",
        _ => "Error"
    };
}
