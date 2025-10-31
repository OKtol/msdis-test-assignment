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

    public async Task<int> GetVacanciesFoundAsync(string searchText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchText);

        var escapedSearchText = searchText.Contains('%') 
            ? searchText 
            : Uri.EscapeDataString(searchText);
        var requestUri = BuildRequestUri(escapedSearchText);

        try
        {
            using var resp = await _http.GetAsync(requestUri);
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

    private Uri BuildRequestUri(string escapedSearchText)
    {
        try
        {
            var ub = new UriBuilder(_baseUrl);
            var existing = ub.Query;
            var currentQuery = string.IsNullOrEmpty(existing) ? "" : existing.TrimStart('?');

            var newQuery = string.IsNullOrEmpty(currentQuery)
                ? $"text={escapedSearchText}"
                : currentQuery + $"&text={escapedSearchText}";

            ub.Query = newQuery;
            return ub.Uri;
        }
        catch (UriFormatException ex)
        {
            throw new InvalidOperationException("Failed to construct HH API request URI", ex);
        }
    }
}
