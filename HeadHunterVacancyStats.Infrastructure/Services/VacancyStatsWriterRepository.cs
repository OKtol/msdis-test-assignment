using Amazon.S3;
using HeadHunterVacancyStats.Domain.Models;
using HeadHunterVacancyStats.Infrastructure.Interfaces;
using System.Collections.Concurrent;
using System.Text.Json;

namespace HeadHunterVacancyStats.Infrastructure.Services;

public class VacancyStatsWriterRepository : IVacancyStatsWriterRepository
{
    private readonly IBaseS3Repository _repo;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private const int InitialCapacity = 101;
    private static readonly int DefaultConcurrencyLevel = Environment.ProcessorCount * 2;
    private readonly ConcurrentDictionary<string, VacancyStat> _stats;

    public VacancyStatsWriterRepository(IBaseS3Repository repo)
    {
        _repo = repo;
        _stats = new ConcurrentDictionary<string, VacancyStat>(DefaultConcurrencyLevel, InitialCapacity);

        try
        {
            InitializeAsync().GetAwaiter().GetResult();
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // First run, empty stats is okay
        }
        catch (AmazonS3Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize stats data", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to process vacancy stats data during initialization", ex);
        }
    }

    private async Task InitializeAsync()
    {
        var json = await _repo.GetObjectStringAsync();
        if (string.IsNullOrEmpty(json))
            return;

        try
        {
            var stats = JsonSerializer.Deserialize<VacancyStat[]>(json, _jsonOptions)
                ?? throw new JsonException("Failed to deserialize vacancy stats: null result");

            if (stats.Length > 0)
                foreach (var stat in stats)
                    _stats.AddOrUpdate(
                        key: stat.Date,
                        addValue: stat,
                        updateValueFactory: (k, old) => stat
                        );
        }
        catch (JsonException) { throw; }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException("Unexpected error initializing stats", ex);
        }
    }

    public async Task SaveDailyStatsAsync(string date, int vacanciesCount)
    {
        ArgumentException.ThrowIfNullOrEmpty(date);
        ArgumentOutOfRangeException.ThrowIfNegative(vacanciesCount);

        try
        {
            var stat = VacancyStat.Create(date, vacanciesCount);

            _stats.AddOrUpdate(
                key: date,
                addValue: stat,
                updateValueFactory: (k, old) => stat);

            await SaveToStorageAsync();
        }
        catch (AmazonS3Exception ex)
        {
            throw new InvalidOperationException($"Failed to save stats data", ex);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException("Failed to process vacancy stats data", ex);
        }
    }

    private async Task SaveToStorageAsync()
    {
        var orderedStats = _stats.Values
            .OrderBy(x => x.Date)
            .ToArray();

        var json = JsonSerializer.Serialize(orderedStats, _jsonOptions);
        await _repo.PutObjectStringAsync(json);
    }
}