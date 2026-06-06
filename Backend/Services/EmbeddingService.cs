using System.Text.Json;
using Pgvector;

namespace Backend.Services;

public interface IEmbeddingService
{
    Task<Vector> GetEmbeddingAsync(string text, CancellationToken ct = default);
}

public class GeminiEmbeddingService(HttpClient http, IConfiguration config) : IEmbeddingService
{
    private readonly string _apiKey = config["Gemini:ApiKey"] is { Length: > 0 } key
        ? key
        : throw new InvalidOperationException("Gemini:ApiKey is not configured.");

    private const string Model = "gemini-embedding-001";

    public async Task<Vector> GetEmbeddingAsync(string text, CancellationToken ct = default)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:embedContent?key={_apiKey}";
        var body = new
        {
            content = new { parts = new[] { new { text } } }
        };
        var response = await http.PostAsJsonAsync(url, body, ct);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Gemini API {(int)response.StatusCode}: {err}");
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        var values = json
            .GetProperty("embedding")
            .GetProperty("values")
            .EnumerateArray()
            .Select(v => v.GetSingle())
            .ToArray();
        return new Vector(values);
    }
}
