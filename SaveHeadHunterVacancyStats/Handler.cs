using HeadHunterVacancyStats.Domain.Models.HttpResponse;
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
        try
        {
            var svc = _serviceProvider.GetRequiredService<SaveVacancyStatsService>();
            var response = await svc.SaveTodayStatsAsync();

            return HttpResponseBuilder.Build(response, 200);
        }
        catch (Exception)
        {
            return HttpResponseBuilder.BuildError("Failed to save vacancy stats", 500);
        }
    }
}
