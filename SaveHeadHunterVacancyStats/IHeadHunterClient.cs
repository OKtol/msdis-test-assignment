
namespace SaveHeadHunterVacancyStats
{
    public interface IHeadHunterClient
    {
        Task<int> GetVacanciesFoundAsync(string searchText);
    }
}