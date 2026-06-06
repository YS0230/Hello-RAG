using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public interface IJiebaDictService
{
    Task RebuildAsync(CancellationToken ct = default);
}

public class JiebaDictService(AppDbContext db, IConfiguration config, ILogger<JiebaDictService> logger)
    : IJiebaDictService
{
    public async Task RebuildAsync(CancellationToken ct = default)
    {
        var dictPath = config["Jieba:DictPath"] ?? "/jieba_dict/user_dict.txt";

        var entries = await db.JiebaDictEntries
            .Where(e => e.IsActive)
            .OrderBy(e => e.Id)
            .ToListAsync(ct);

        var lines = entries.Select(e => $"{e.Word} {e.Frequency} {e.PartOfSpeech}");
        var dir = Path.GetDirectoryName(dictPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        await File.WriteAllLinesAsync(dictPath, lines, ct);
        logger.LogInformation("Wrote {Count} jieba dict entries to {Path}", entries.Count, dictPath);

        await db.Database.ExecuteSqlRawAsync(
            "SELECT jieba_reload_dict()", ct);

        await db.Database.ExecuteSqlRawAsync(
            """UPDATE "SystemRules" SET "UpdatedAt" = NOW()""", ct);

        logger.LogInformation("Jieba dict reloaded and SearchVector rebuilt.");
    }
}
