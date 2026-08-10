using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

public class LogActionFilter : IAsyncActionFilter
{
    private readonly ILogger<LogActionFilter> _logger;

    public LogActionFilter(ILogger<LogActionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var action = context.ActionDescriptor.DisplayName;
        var user = context.HttpContext.User.Identity?.Name ?? "anonymous";
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Starting {Action} by {User}", action, user);
        var executed = await next(); // runs the action (+ later filters)
        sw.Stop();
        _logger.LogInformation(
            "Finished {Action} in {ElapsedMs}ms, status exception={HasException}",
            action,
            sw.ElapsedMilliseconds,
            executed.Exception is not null);
    }
}