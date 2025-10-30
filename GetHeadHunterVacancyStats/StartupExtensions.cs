using Microsoft.Extensions.DependencyInjection;

namespace GetHeadHunterVacancyStats;

public static class StartupExtensions
{
    public static IServiceCollection AddGetVacancyServices(this IServiceCollection services)
    {
        services.AddSingleton<GetVacancyStatsService>();
        return services;
    }
}