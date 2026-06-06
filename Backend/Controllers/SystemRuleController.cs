using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemRuleController(AppDbContext db, IEmbeddingService embedding) : ControllerBase
{
    private static SystemRuleDto ToDto(SystemRule r) => new()
    {
        Id = r.Id,
        SystemName = r.SystemName,
        SerialNumber = r.SerialNumber,
        System = r.System,
        RuleDescription = r.RuleDescription,
        Recorder = r.Recorder,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
    };

    [HttpGet]
    public async Task<IEnumerable<SystemRuleDto>> GetAll() =>
        (await db.SystemRules.OrderByDescending(r => r.CreatedAt).ToListAsync()).Select(ToDto);

    [HttpGet("{id}")]
    public async Task<ActionResult<SystemRuleDto>> GetById(int id)
    {
        var rule = await db.SystemRules.FindAsync(id);
        return rule is null ? NotFound() : ToDto(rule);
    }

    private static string EmbeddingText(string systemName, string system, string ruleDescription) =>
        $"SystemName: {systemName} System: {system} RuleDescription: {ruleDescription}";

    [HttpPost]
    public async Task<ActionResult<SystemRuleDto>> Create(SystemRuleDto dto)
    {
        var rule = new SystemRule
        {
            SystemName = dto.SystemName,
            SerialNumber = dto.SerialNumber,
            System = dto.System,
            RuleDescription = dto.RuleDescription,
            Recorder = dto.Recorder,
        };
        rule.Embedding = await embedding.GetEmbeddingAsync(
            EmbeddingText(rule.SystemName, rule.System, rule.RuleDescription));
        db.SystemRules.Add(rule);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = rule.Id }, ToDto(rule));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, SystemRuleDto dto)
    {
        var rule = await db.SystemRules.FindAsync(id);
        if (rule is null) return NotFound();

        bool needsReEmbed = rule.SystemName != dto.SystemName
            || rule.System != dto.System
            || rule.RuleDescription != dto.RuleDescription;

        rule.SystemName = dto.SystemName;
        rule.SerialNumber = dto.SerialNumber;
        rule.System = dto.System;
        rule.RuleDescription = dto.RuleDescription;
        rule.Recorder = dto.Recorder;
        rule.UpdatedAt = DateTime.UtcNow;

        if (needsReEmbed)
            rule.Embedding = await embedding.GetEmbeddingAsync(
                EmbeddingText(rule.SystemName, rule.System, rule.RuleDescription));

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var rule = await db.SystemRules.FindAsync(id);
        if (rule is null) return NotFound();
        db.SystemRules.Remove(rule);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
