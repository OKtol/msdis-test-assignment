using HeadHunterVacancyStats.Domain.Models;

namespace HeadHunterVacancyStats.Infrastructure.Interfaces;

public interface IVacancyStatsWriterRepository
{
    Task SaveDailyStatsAsync(VacancyStat vacancyStat);
}