namespace MyBackend.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("HTTP Request: {Method} {Path}", context.Request.Method, context.Request.Path);
        
        await next(context);
        
        logger.LogInformation("HTTP Response: {StatusCode} for {Path}", context.Response.StatusCode, context.Request.Path);
    }
}