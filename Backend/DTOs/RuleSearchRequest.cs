namespace Backend.DTOs;

public class RuleSearchRequest
{
    public string Query { get; set; } = string.Empty;
    public int TopN { get; set; } = 5;
}
