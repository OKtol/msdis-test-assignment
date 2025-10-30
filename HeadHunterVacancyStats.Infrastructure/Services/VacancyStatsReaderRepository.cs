using Amazon.S3;
using HeadHunterVacancyStats.Domain.Models;
using HeadHunterVacancyStats.Infrastructure.Interfaces;
using System.Text.Json;

namespace HeadHunterVacancyStats.Infrastructure.Services;

public class VacancyStatsReaderRepository : IVacancyStatsReaderRepository
{
    private readonly IBaseS3Repository _repo;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public VacancyStatsReaderRepository(IBaseS3Repository repo)
    {
        _repo = repo;
    }

    public async Task<VacancyStat[]> GetStatsAsync()
    {
        try
        {
            var json = await _repo.GetObjectStringAsync();
            if (string.IsNullOrEmpty(json))
                return [];

            var stats = JsonSerializer.Deserialize<VacancyStat[]>(json, _jsonOptions);
            return stats ?? [];
        }
        catch (AmazonS3Exception ex)
        {
            throw new InvalidOperationException($"Failed to access stats data", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to process vacancy stats data", ex);
        }
    }
}