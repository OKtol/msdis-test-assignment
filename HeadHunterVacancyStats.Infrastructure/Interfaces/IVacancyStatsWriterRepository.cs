namespace HeadHunterVacancyStats.Infrastructure.Interfaces
{
    public interface IVacancyStatsWriterRepository
    {
        Task SaveDailyStatsAsync(string date, int vacanciesCount);
    }
}