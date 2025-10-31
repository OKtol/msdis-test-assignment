using HeadHunterVacancyStats.Domain.Models;

namespace HeadHunterVacancyStats.Infrastructure.Interfaces;

public interface IVacancyStatsReaderRepository
{
    Task<VacancyStat[]> GetStatsAsync();
}