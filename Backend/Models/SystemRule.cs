using Pgvector;

namespace Backend.Models;

public class SystemRule
{
    public int Id { get; set; }
    public string SystemName { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string System { get; set; } = string.Empty;
    public string RuleDescription { get; set; } = string.Empty;
    public string Recorder { get; set; } = string.Empty;
    public Vector? Embedding { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
