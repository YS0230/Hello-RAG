using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JiebaDictController(AppDbContext db, IJiebaDictService jieba) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JiebaDictEntryDto>>> GetAll(CancellationToken ct)
    {
        var list = await db.JiebaDictEntries
            .OrderBy(e => e.Word)
            .Select(e => ToDto(e))
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<JiebaDictEntryDto>> Create(JiebaDictEntryPayload payload, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(payload.Word))
            return BadRequest("Word is required.");

        var entry = new JiebaDictEntry
        {
            Word = payload.Word.Trim(),
            Frequency = payload.Frequency,
            PartOfSpeech = payload.PartOfSpeech,
            IsActive = payload.IsActive,
        };
        db.JiebaDictEntries.Add(entry);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetAll), ToDto(entry));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, JiebaDictEntryPayload payload, CancellationToken ct)
    {
        var entry = await db.JiebaDictEntries.FindAsync([id], ct);
        if (entry is null) return NotFound();

        entry.Word = payload.Word.Trim();
        entry.Frequency = payload.Frequency;
        entry.PartOfSpeech = payload.PartOfSpeech;
        entry.IsActive = payload.IsActive;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var entry = await db.JiebaDictEntries.FindAsync([id], ct);
        if (entry is null) return NotFound();
        db.JiebaDictEntries.Remove(entry);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("rebuild")]
    public async Task<IActionResult> Rebuild(CancellationToken ct)
    {
        await jieba.RebuildAsync(ct);
        return Ok(new { message = "詞典已重新載入，SearchVector 重建完成。" });
    }

    private static JiebaDictEntryDto ToDto(JiebaDictEntry e) => new()
    {
        Id = e.Id,
        Word = e.Word,
        Frequency = e.Frequency,
        PartOfSpeech = e.PartOfSpeech,
        IsActive = e.IsActive,
        CreatedAt = e.CreatedAt,
    };
}
