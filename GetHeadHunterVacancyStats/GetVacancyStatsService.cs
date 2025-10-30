using HeadHunterVacancyStats.Domain.Models;
using HeadHunterVacancyStats.Infrastructure.Interfaces;

namespace GetHeadHunterVacancyStats;

public class GetVacancyStatsService
{
    private readonly IVacancyStatsReaderRepository _repository;

    public GetVacancyStatsService(IVacancyStatsReaderRepository repository)
    {
        _repository = repository;
    }

    public async Task<VacancyStat[]> GetStatsAsync()
    {
        try
        {
            return await _repository.GetStatsAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to retrieve vacancy stats", ex);
        }
    }
}