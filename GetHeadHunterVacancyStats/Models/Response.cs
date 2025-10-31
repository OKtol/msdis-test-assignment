using HeadHunterVacancyStats.Domain.Models;

namespace GetHeadHunterVacancyStats.Models;

public class Response
{
    public required string Message { get; set; }

    public required VacancyStat[] Vacancies { get; set; }
}
