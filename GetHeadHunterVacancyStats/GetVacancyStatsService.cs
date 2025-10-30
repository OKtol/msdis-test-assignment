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

    public Task<VacancyStat[]> GetStatsAsync()
    {
        return _repository.GetStatsAsync();
    }
}