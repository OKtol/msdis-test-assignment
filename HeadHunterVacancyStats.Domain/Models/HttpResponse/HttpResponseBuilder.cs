using System.Text.Json;

namespace HeadHunterVacancyStats.Domain.Models.HttpResponse;

public static class HttpResponseBuilder
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public static string Build(object bodyObject, int statusCode = 200, IDictionary<string, string>? headers = null)
    {
        var bodyJson = JsonSerializer.Serialize(bodyObject, _jsonOptions);

        var envelope = new HttpResponseEnvelope
        {
            IsBase64Encoded = false,
            StatusCode = statusCode,
            Headers = headers ?? new Dictionary<string, string> { ["Content-Type"] = "application/json" },
            Body = bodyJson
        };

        return JsonSerializer.Serialize(envelope, _jsonOptions);
    }

    public static string BuildError(string message, int statusCode = 500)
    {
        var payload = new { error = message };
        return Build(payload, statusCode);
    }
}
