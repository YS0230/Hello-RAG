using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class AiSearchRequest
{
    [Required]
    public string Query { get; set; } = string.Empty;
    public int? TopN { get; set; }
}

public class AiSearchResponse
{
    public string Answer { get; set; } = string.Empty;
    public List<RuleSearchResultDto> Sources { get; set; } = [];
}
