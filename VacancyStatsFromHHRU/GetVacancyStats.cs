namespace VacancyStatsFromHeadHunter;

public class GetVacancyStats
{
    private const string Key = "vacancies_stats.json";

    public static async Task<string> RunAsync()
    {
        var bucket = Environment.GetEnvironmentVariable("S3_BUCKET") ?? throw new InvalidOperationException("S3_BUCKET missing");
        var content = await S3Helper.GetObjectStringAsync(bucket, Key);
        if (string.IsNullOrEmpty(content))
            return "[]"; // вернуть пустой массив JSON
        return content;
    }
}
