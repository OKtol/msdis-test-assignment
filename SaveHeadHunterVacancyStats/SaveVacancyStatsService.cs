using HeadHunterVacancyStats.Infrastructure.Interfaces;

namespace SaveHeadHunterVacancyStats;

public class SaveVacancyStatsService
{
    private readonly IHeadHunterClient _client;
    private readonly IVacancyStatsWriterRepository _repository;

    public SaveVacancyStatsService(IHeadHunterClient client, IVacancyStatsWriterRepository repository)
    {
        _client = client;
        _repository = repository;
    }

    public async Task<string> SaveTodayStatsAsync()
    {
        try
        {
            var vacanciesCount = await _client.GetCSharpVacanciesFoundAsync();
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            
            await _repository.SaveDailyStatsAsync(today, vacanciesCount);
            
            return $"OK: {today} -> {vacanciesCount}";
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save vacancy stats", ex);
        }
    }
}