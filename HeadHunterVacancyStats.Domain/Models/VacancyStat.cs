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
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            if (!DateTime.TryParseExact(value, DateFormat, DateFormatInfo,
                DateTimeStyles.None, out _))
            {
                throw new ArgumentException($"Date must be in format {DateFormat}", nameof(value));
            }
            _date = value;
        }
    }

    public required Jobs Vacancies { get; init; }
}