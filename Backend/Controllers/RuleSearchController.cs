using Backend.Data;
using Backend.DTOs;
using Backend.Queries;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RuleSearchController(AppDbContext db, IEmbeddingService embedding) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<IEnumerable<RuleSearchResultDto>>> Search(
        RuleSearchRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest("Query is required.");

        var topN = request.TopN is > 0 and <= 20 ? request.TopN : 5;
        var queryVector = await embedding.GetEmbeddingAsync(request.Query, ct);
        var floats = queryVector.Memory.ToArray();

        var sql = RuleSearchQuery.Build(floats, request.Query, topN);

        var results = await db.Database
            .SqlQuery<RuleSearchResultDto>(sql)
            .ToListAsync(ct);

        return Ok(results);
    }
}
