using Backend.Services;

namespace Backend.Middleware;

public class RateLimitMiddleware(RequestDelegate next, IConfiguration config, RateLimitCounterService counter)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var allowedIps = config.GetSection("RateLimit:AllowedIps").Get<string[]>() ?? [];
        var dailyLimit = config.GetValue<int>("RateLimit:DailyLimit", 100);

        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        string ip;
        if (!string.IsNullOrEmpty(forwarded))
        {
            ip = forwarded.Split(',')[0].Trim();
        }
        else
        {
            var remote = context.Connection.RemoteIpAddress;
            ip = remote?.IsIPv4MappedToIPv6 == true
                ? remote.MapToIPv4().ToString()
                : remote?.ToString() ?? "unknown";
        }

        if (allowedIps.Contains(ip))
        {
            await next(context);
            return;
        }

        var entry = counter.Increment(ip);

        if (entry.Count > dailyLimit)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"message\":\"已超過每日存取上限，請明日再試。\"}");
            return;
        }

        await next(context);
    }
}
