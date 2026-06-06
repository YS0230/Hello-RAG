using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserActionLogController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Query(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? resource = null,
        [FromQuery] string? action = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? ip = null)
    {
        var query = db.UserActionLogs.AsQueryable();

        if (!string.IsNullOrEmpty(resource))
            query = query.Where(l => l.Resource == resource);
        if (!string.IsNullOrEmpty(action))
            query = query.Where(l => l.Action == action);
        if (from.HasValue)
            query = query.Where(l => l.Timestamp >= from.Value);
        if (to.HasValue)
            query = query.Where(l => l.Timestamp <= to.Value);
        if (!string.IsNullOrEmpty(ip))
            query = query.Where(l => l.IpAddress != null && l.IpAddress.Contains(ip));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new
            {
                l.Id,
                l.Timestamp,
                l.Action,
                l.Resource,
                l.ResourceId,
                l.HttpMethod,
                l.RequestPath,
                l.StatusCode,
                l.IpAddress,
            })
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }
}
