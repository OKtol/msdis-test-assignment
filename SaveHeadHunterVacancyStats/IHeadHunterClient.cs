
namespace SaveHeadHunterVacancyStats
{
    public interface IHeadHunterClient
    {
        Task<int> GetCSharpVacanciesFoundAsync();
    }
}