namespace AspnetCoreMsDocs.Learn.StartupFilters;

public class RequestHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestHandlingMiddleware> _logger;

    public RequestHandlingMiddleware(RequestDelegate next, ILogger<RequestHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var option = context.Request.Query["option"];
        if (string.IsNullOrWhiteSpace((option)))
        {
            _logger.LogInformation($"QueryString from request: {0}", option);
        }
        await _next(context);
    }
}