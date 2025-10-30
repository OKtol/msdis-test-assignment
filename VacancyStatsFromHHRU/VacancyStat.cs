namespace VacancyStatsFromHeadHunter;

public class VacancyStat
{
    public required string Date { get; set; }
    public int Vacancies { get; set; }

    public static VacancyStat Create(int vacancies) => new()
    {
        Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
        Vacancies = vacancies
    };
}