namespace HeadHunterVacancyStats.Infrastructure.Interfaces;

public interface IBaseS3Repository
{
    Task<string?> GetObjectStringAsync();
    Task PutObjectStringAsync(string content);
}