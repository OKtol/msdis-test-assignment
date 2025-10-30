using System.Globalization;

namespace HeadHunterVacancyStats.Domain.Models;

public class VacancyStat
{
    private const string DateFormat = "yyyy-MM-dd";
    private static readonly DateTimeFormatInfo DateFormatInfo = DateTimeFormatInfo.InvariantInfo;
    private string _date = string.Empty;

    public string Date
    {
        get => _date;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Date cannot be empty", nameof(value));

            if (!DateTime.TryParseExact(value, DateFormat, DateFormatInfo,
                DateTimeStyles.None, out _))
            {
                throw new ArgumentException($"Date must be in format {DateFormat}", nameof(value));
            }
            _date = value;
        }
    }

    public int Vacancies { get; init; }

    public static VacancyStat Create(string date, int vacancies)
    {
        if (vacancies < 0)
            throw new ArgumentException("Vacancies count cannot be negative", nameof(vacancies));

        return new VacancyStat
        {
            Date = date,
            Vacancies = vacancies
        };
    }
}