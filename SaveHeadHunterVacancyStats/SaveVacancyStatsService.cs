using HeadHunterVacancyStats.Domain.Attributes;
using HeadHunterVacancyStats.Domain.Models;
using HeadHunterVacancyStats.Infrastructure.Interfaces;
using SaveHeadHunterVacancyStats.Models;
using System.Reflection;

namespace SaveHeadHunterVacancyStats;

public class SaveVacancyStatsService
{
    private readonly IHeadHunterClient _client;
    private readonly IVacancyStatsWriterRepository _repository;

    public SaveVacancyStatsService(IHeadHunterClient client, IVacancyStatsWriterRepository repository)
    {
        _client = client;
        _repository = repository;
    }

    public async Task<Response> SaveTodayStatsAsync()
    {
        try
        {
            var props = GetJobsSearchableProperties();
            var results = await FetchCountsAsync(props);
            var jobs = BuildJobs(results);
            var vacancyStat = BuildVacancyStat(jobs);

            await _repository.SaveDailyStatsAsync(vacancyStat);

            // 5. возвращаем структуру, соответствующую примеру JSON
            var response = new Response
            {
                Message = "Data saved successfully",
                Vacancies = vacancyStat 
            };

            return response;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save vacancy stats", ex);
        }
    }

    private static PropertyInfo[] GetJobsSearchableProperties()
    {
        var props = typeof(Jobs)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<SearchTextAttribute>() is not null)
            .ToArray();

        if (props.Length == 0)
            throw new InvalidOperationException("No searchable job properties found on Jobs model.");

        return props;
    }

    private Task<(PropertyInfo prop, int count)[]> FetchCountsAsync(PropertyInfo[] props)
    {
        var tasks = props.Select(async prop =>
        {
            var attr = prop.GetCustomAttribute<SearchTextAttribute>()!;
            var text = attr.Text ?? throw new InvalidOperationException($"SearchText missing on {prop.Name}");
            var encoded = text.Contains('%') ? text : Uri.EscapeDataString(text);
            var count = await _client.GetVacanciesFoundAsync(encoded);
            return (prop, count);
        });

        return Task.WhenAll(tasks);
    }

    private static Jobs BuildJobs((PropertyInfo prop, int count)[] data)
    {
        var jobs = Activator.CreateInstance<Jobs>() ??
                   throw new InvalidOperationException("Unable to create Jobs instance.");

        foreach (var (prop, count) in data)
        {
            if (!prop.CanWrite)
                throw new InvalidOperationException($"Property {prop.Name} is read-only.");

            if (prop.PropertyType == typeof(int))
                prop.SetValue(jobs, count);
            else
                prop.SetValue(jobs, Convert.ChangeType(count, prop.PropertyType));
        }

        return jobs;
    }

    private static VacancyStat BuildVacancyStat(Jobs jobs)
    {
        return new VacancyStat
        {
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            Vacancies = jobs
        };
    }
}