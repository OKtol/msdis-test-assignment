using HeadHunterVacancyStats.Domain.Models.HttpResponse;
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

    public async Task<string> FunctionHandler(string input)
    {
        try
        {
            var service = _serviceProvider.GetRequiredService<GetVacancyStatsService>();
            var response = await service.GetStatsAsync();

            return HttpResponseBuilder.Build(response, statusCode: 200);
        }
        catch (Exception ex)
        {
            return HttpResponseBuilder.BuildError(ex.Message);
        }
    }
}
