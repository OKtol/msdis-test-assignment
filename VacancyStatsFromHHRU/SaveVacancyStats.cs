using System.Text.Json;

namespace VacancyStatsFromHeadHunter;

public class SaveVacancyStats
{
    private const string Key = "vacancies_stats.json";

    public static async Task<string> RunAsync()
    {
        var bucket = Environment.GetEnvironmentVariable("S3_BUCKET") ?? throw new InvalidOperationException("S3_BUCKET missing");
        var found = await HeadHunterClient.GetCSharpVacanciesFoundAsync();
        var existingJson = await S3Helper.GetObjectStringAsync(bucket, Key);

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

        List<VacancyStat> list;
        if (string.IsNullOrEmpty(existingJson))
            list = new List<VacancyStat>();
        else
            list = JsonSerializer.Deserialize<List<VacancyStat>>(existingJson, options) ?? new List<VacancyStat>();

        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var item = list.FirstOrDefault(x => x.Date == today);
        if (item != null)
        {
            item.Vacancies = found;
        }
        else
        {
            list.Add(new VacancyStat { Date = today, Vacancies = found });
            list = list.OrderBy(x => x.Date).ToList();
        }

        var outJson = JsonSerializer.Serialize(list, options);
        await S3Helper.PutObjectStringAsync(bucket, Key, outJson);

        return $"OK: {today} -> {found}";
    }
}
