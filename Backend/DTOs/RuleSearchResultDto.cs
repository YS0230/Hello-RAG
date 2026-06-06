namespace Backend.DTOs;

public class RuleSearchResultDto
{
    public int Id { get; set; }
    public string SystemName { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string System { get; set; } = string.Empty;
    public string RuleDescription { get; set; } = string.Empty;
    public string Recorder { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public double Similarity { get; set; }
    public double VectorSimilarity { get; set; }
    public double TextScore { get; set; }
    public string? SearchVector { get; set; }
}
