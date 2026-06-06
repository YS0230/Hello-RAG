using System.Text.Encodings.Web;
using System.Text.Json;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.Filters;

public class ActionLoggingFilter(AppDbContext db) : IAsyncActionFilter
{
    private static readonly HashSet<string> SkipPaths =
        ["useractionlog"];

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method;
        if (method is not ("GET" or "POST" or "PUT" or "DELETE"))
        {
            await next();
            return;
        }

        var path = context.HttpContext.Request.Path.Value ?? string.Empty;
        if (SkipPaths.Any(s => path.Contains(s, StringComparison.OrdinalIgnoreCase)))
        {
            await next();
            return;
        }

        var executed = await next();

        var statusCode = context.HttpContext.Response.StatusCode;
        if (statusCode < 200 || statusCode >= 300)
            return;

        var resourceId = context.RouteData.Values["id"]?.ToString();
        var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
        var ip = remoteIp?.IsIPv4MappedToIPv6 == true
            ? remoteIp.MapToIPv4().ToString()
            : remoteIp?.ToString();

        string? payload = null;
        if (context.ActionArguments.Count > 0)
        {
            var args = context.ActionArguments
                .Where(kv => kv.Key != "id" && kv.Value is not CancellationToken)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            if (args.Count > 0)
            {
                try
                {
                    payload = JsonSerializer.Serialize(args, new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    });
                }
                catch
                {
                    payload = string.Join("; ", args.Select(kv => $"{kv.Key}={kv.Value}"));
                }
            }
        }

        var action = method switch
        {
            "GET" => "Read",
            "POST" => "Create",
            "PUT" => "Update",
            "DELETE" => "Delete",
            _ => method
        };

        db.UserActionLogs.Add(new UserActionLog
        {
            Action = action,
            Resource = payload,
            ResourceId = resourceId,
            HttpMethod = method,
            RequestPath = path,
            StatusCode = statusCode,
            IpAddress = ip,
        });

        await db.SaveChangesAsync();
    }
}
