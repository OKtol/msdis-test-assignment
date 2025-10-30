using System.Text.Json;

namespace SaveHeadHunterVacancyStats;

public class HeadHunterClient : IHeadHunterClient
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public HeadHunterClient(HttpClient http, string baseUrl)
    {
        ArgumentNullException.ThrowIfNull(baseUrl);

        _http = http;
        _baseUrl = baseUrl;
    }

    public async Task<int> GetCSharpVacanciesFoundAsync()
    {
        try
        {
            using var resp = await _http.GetAsync(_baseUrl);

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"HH API request failed: {resp.StatusCode}");

            using var stream = await resp.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            if (doc.RootElement.TryGetProperty("found", out var foundElement) &&
                foundElement.TryGetInt32(out var found))
                return found;

            throw new InvalidOperationException("Response does not contain 'found'.");
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException("Failed to get vacancy count from HH API", ex);
        }
    }
}
