using Backend.Data;
using Backend.DTOs;
using Backend.Queries;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiSearchController(AppDbContext db, IEmbeddingService embedding, IAiAnswerService aiAnswer) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AiSearchResponse>> Search(AiSearchRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest("Query is required.");

        var queryVector = await embedding.GetEmbeddingAsync(request.Query, ct);
        var floats = queryVector.Memory.ToArray();

        var sql = RuleSearchQuery.Build(floats, request.Query, topN: 3);

        var sources = await db.Database
            .SqlQuery<RuleSearchResultDto>(sql)
            .ToListAsync(ct);

        if (sources.Count == 0)
            return Ok(new AiSearchResponse { Answer = "知識庫中找不到與您的問題相關的資料。", Sources = [] });

        var answer = await aiAnswer.GenerateAnswerAsync(request.Query, sources, ct);

        return Ok(new AiSearchResponse { Answer = answer, Sources = sources });
    }
}
