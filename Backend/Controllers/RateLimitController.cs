using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RateLimitController(RateLimitCounterService counter) : ControllerBase
{
    [HttpGet("daily")]
    public IActionResult GetDailyCounts()
    {
        var items = counter.GetTodayCounts()
            .Select(x => new { ip = x.Ip, count = x.Count });
        return Ok(items);
    }
}
