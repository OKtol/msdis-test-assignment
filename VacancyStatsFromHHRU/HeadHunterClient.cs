using System.Text.Json;

namespace VacancyStatsFromHeadHunter;

public static class HeadHunterClient
{
    private static readonly HttpClient _http = new HttpClient();

    public static async Task<int> GetCSharpVacanciesFoundAsync()
    {
        var url = Environment.GetEnvironmentVariable("HH_API_URL")
                  ?? "https://api.hh.ru/vacancies?text=C%23%20Developer&schedule=remote&per_page=100";

        using var resp = await _http.GetAsync(url);
        resp.EnsureSuccessStatusCode();
        using var stream = await resp.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        if (doc.RootElement.TryGetProperty("found", out var foundEl) && foundEl.TryGetInt32(out var found))
            return found;
        throw new InvalidOperationException("Response does not contain 'found'.");
    }
}
