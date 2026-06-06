namespace Backend.Models;

public class JiebaDictEntry
{
    public int Id { get; set; }
    public string Word { get; set; } = string.Empty;
    public int Frequency { get; set; } = 100;
    public string PartOfSpeech { get; set; } = "n";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
