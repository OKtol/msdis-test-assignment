using HeadHunterVacancyStats.Domain.Models;

namespace SaveHeadHunterVacancyStats.Models;

public class Response
{
    public required string Message { get; set; }

    public required VacancyStat Vacancies { get; set; }
}
