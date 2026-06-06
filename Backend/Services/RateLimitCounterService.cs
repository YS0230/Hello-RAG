using System.Collections.Concurrent;

namespace Backend.Services;

public class RateLimitCounterService
{
    private readonly ConcurrentDictionary<string, (int Count, DateOnly Date)> _counters = new();

    public (int Count, DateOnly Date) Increment(string ip)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return _counters.AddOrUpdate(
            ip,
            _ => (1, today),
            (_, old) => old.Date == today ? (old.Count + 1, today) : (1, today));
    }

    public IEnumerable<(string Ip, int Count)> GetTodayCounts()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return _counters
            .Where(kv => kv.Value.Date == today)
            .Select(kv => (kv.Key, kv.Value.Count))
            .OrderByDescending(x => x.Count);
    }
}
