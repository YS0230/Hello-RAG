namespace Backend.DTOs;

public class SystemRuleDto
{
    public int Id { get; set; }
    public string SystemName { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string System { get; set; } = string.Empty;
    public string RuleDescription { get; set; } = string.Empty;
    public string Recorder { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
