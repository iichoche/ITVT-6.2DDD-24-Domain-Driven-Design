public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string HEADER = "X-API-KEY";

    public ApiKeyMiddleware(RequestDelegate next)
        => _next = next;

    public async Task InvokeAsync(HttpContext context, IConfiguration config)
    {
        // 1) Kijk of de header mee komt
        if (!context.Request.Headers.TryGetValue(HEADER, out var providedKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API key is missing");
            return;
        }

        // 2) Vergelijk met de key uit configuratie (of Key Vault)
        var configuredKey = config["ApiKey"];
        if (string.IsNullOrEmpty(configuredKey) || providedKey != configuredKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid API key");
            return;
        }

        // 3) Alles oké, door naar de volgende middleware/controller
        await _next(context);
    }
}
