using Amazon.S3;
using HeadHunterVacancyStats.Infrastructure.Services;
using HeadHunterVacancyStats.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HeadHunterVacancyStats.Infrastructure;

public static class StartupExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var accessKey = Environment.GetEnvironmentVariable("S3_ACCESS_KEY")
                ?? throw new InvalidOperationException("S3_ACCESS_KEY missing");
            var secretKey = Environment.GetEnvironmentVariable("S3_SECRET_KEY")
                ?? throw new InvalidOperationException("S3_SECRET_KEY missing");
            var serviceUrl = Environment.GetEnvironmentVariable("S3_URL")
                ?? throw new InvalidOperationException("S3_URL missing");

            return new AmazonS3Client(accessKey, secretKey, new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                ForcePathStyle = true
            });
        });

        services.AddSingleton<IBaseS3Repository>(sp =>
        {
            var bucket = Environment.GetEnvironmentVariable("S3_BUCKET")
                ?? throw new InvalidOperationException("S3_BUCKET missing");

            return new BaseS3Repository(bucket, sp.GetRequiredService<IAmazonS3>());
        });

        services.AddSingleton<IVacancyStatsReaderRepository, VacancyStatsReaderRepository>();
        services.AddSingleton<IVacancyStatsWriterRepository, VacancyStatsWriterRepository>();

        return services;
    }
}