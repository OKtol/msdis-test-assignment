using HeadHunterVacancyStats.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace SaveHeadHunterVacancyStats;

public class Handler
{
    private readonly IServiceProvider _serviceProvider;

    public Handler()
    {
        var services = new ServiceCollection();

        services.AddInfrastructure();
        services.AddSaveVacancyServices(); 

        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task<string> FunctionHandler(string input)
    {
        var service = _serviceProvider.GetRequiredService<SaveVacancyStatsService>();
        return await service.SaveTodayStatsAsync();
    }
}
