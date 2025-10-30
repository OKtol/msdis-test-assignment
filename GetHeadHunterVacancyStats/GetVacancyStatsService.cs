using HeadHunterVacancyStats.Infrastructure.Interfaces;
using System.Text.Json;

namespace GetHeadHunterVacancyStats;

public class GetVacancyStatsService
{
    private readonly IVacancyStatsReaderRepository _repository;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetVacancyStatsService(IVacancyStatsReaderRepository repository)
    {
        _repository = repository;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<string> GetStatsAsync()
    {
        try
        {
            var stats = await _repository.GetStatsAsync();
            return JsonSerializer.Serialize(stats, _jsonOptions);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to retrieve vacancy stats", ex);
        }
    }
}