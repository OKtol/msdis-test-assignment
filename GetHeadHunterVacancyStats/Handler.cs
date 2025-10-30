using HeadHunterVacancyStats.Domain.Models;
using HeadHunterVacancyStats.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace GetHeadHunterVacancyStats;

public class Handler
{
    private readonly IServiceProvider _serviceProvider;

    public Handler()
    {
        var services = new ServiceCollection();
        
        services.AddInfrastructure();
        services.AddGetVacancyServices();

        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task<VacancyStat[]> FunctionHandler(string input)
    {
        var service = _serviceProvider.GetRequiredService<GetVacancyStatsService>();
        return await service.GetStatsAsync();
    }
}
