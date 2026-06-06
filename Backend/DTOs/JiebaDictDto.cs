namespace Backend.DTOs;

public class JiebaDictEntryDto
{
    public int Id { get; set; }
    public string Word { get; set; } = string.Empty;
    public int Frequency { get; set; }
    public string PartOfSpeech { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class JiebaDictEntryPayload
{
    public string Word { get; set; } = string.Empty;
    public int Frequency { get; set; } = 100;
    public string PartOfSpeech { get; set; } = "n";
    public bool IsActive { get; set; } = true;
}
