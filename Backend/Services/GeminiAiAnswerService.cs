using System.Text.Json;
using Backend.DTOs;

namespace Backend.Services;

public interface IAiAnswerService
{
    Task<string> GenerateAnswerAsync(string query, IEnumerable<RuleSearchResultDto> context, CancellationToken ct = default);
}

public class GeminiAiAnswerService(HttpClient http, IConfiguration config) : IAiAnswerService
{
    private readonly string _apiKey = config["Gemini:ApiKey"] is { Length: > 0 } key
        ? key
        : throw new InvalidOperationException("Gemini:ApiKey is not configured.");

    private const string Model = "gemini-3.1-flash-lite";

    public async Task<string> GenerateAnswerAsync(string query, IEnumerable<RuleSearchResultDto> context, CancellationToken ct = default)
    {
        var rules = context.ToList();
        var contextText = string.Join("\n\n", rules.Select((r, i) =>
            $"[規則 {i + 1}]\n系統名稱：{r.SystemName}\n系統：{r.System}\n規則說明：{r.RuleDescription}"));

        var prompt = $"""
                        你是一位專業的知識庫助手，請以繁體中文回答使用者問題。
                    
                        === 知識庫資料 ===
                        {contextText}
                    
                        === 回答規則 ===
                        1. 先判斷哪些知識庫條目與問題「直接相關」
                        2. 僅根據直接相關的條目回答，忽略無關條目
                        3. 回答開頭請註明：依據「[系統名稱]」的規則
                        4. 若無任何條目與問題相關，請回覆「查無相關規則」
                        5. 若資料不足以完整回答，請如實說明
                    
                        === 使用者問題 ===
                        {query}
                        """;

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={_apiKey}";
        var body = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var response = await http.PostAsJsonAsync(url, body, ct);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Gemini API {(int)response.StatusCode}: {err}");
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        return json
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }
}
