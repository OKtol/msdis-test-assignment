using GetHeadHunterVacancyStats.Models;
using HeadHunterVacancyStats.Infrastructure.Interfaces;

namespace GetHeadHunterVacancyStats;

public class GetVacancyStatsService
{
    private readonly IVacancyStatsReaderRepository _repository;

    public GetVacancyStatsService(IVacancyStatsReaderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Response> GetStatsAsync()
    {
        var vacansyStats = await _repository.GetStatsAsync();

        return new Response
        {
            Message = "Data load successfully",
            Vacancies = vacansyStats
        };
    }
}