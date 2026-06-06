namespace Backend.Models;

public class UserActionLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Action { get; set; } = string.Empty;      // Read / Create / Update / Delete
    public string? Resource { get; set; }                    // 使用者傳入的資料（JSON）
    public string? ResourceId { get; set; }
    public string HttpMethod { get; set; } = string.Empty;
    public string RequestPath { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string? IpAddress { get; set; }
}
