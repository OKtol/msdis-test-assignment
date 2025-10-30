using Microsoft.Extensions.DependencyInjection;

namespace SaveHeadHunterVacancyStats;

public static class StartupExtensions
{
    public static IServiceCollection AddSaveVacancyServices(this IServiceCollection services)
    {
        var baseUrl = Environment.GetEnvironmentVariable("HH_API_URL")
            ?? throw new InvalidOperationException("HH_API_URL missing");

        services.AddSingleton<IHeadHunterClient>(sp =>
        {
            var httpHandler = new SocketsHttpHandler()
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
                MaxConnectionsPerServer = 20
            };

            var client = new HttpClient(httpHandler);
            client.DefaultRequestHeaders.Add("User-Agent", "TestApp/1.0 (oktol.hz@gmail.com)");
            client.Timeout = TimeSpan.FromSeconds(30);

            return new HeadHunterClient(client, baseUrl);
        });

        services.AddSingleton<SaveVacancyStatsService>();

        return services;
    }
}