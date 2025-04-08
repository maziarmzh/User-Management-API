public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log the HTTP method and request path
        Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");

        await _next(context);

        // Log the response status code
        Console.WriteLine($"Response Status: {context.Response.StatusCode}");
    }
}