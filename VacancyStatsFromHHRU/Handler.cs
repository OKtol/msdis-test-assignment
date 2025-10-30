namespace VacancyStatsFromHeadHunter;

public class Handler
{
    public static async Task<string> FunctionHandler(object input)
    {
        var result = await SaveVacancyStats.RunAsync();
        return result; 
    }
}
