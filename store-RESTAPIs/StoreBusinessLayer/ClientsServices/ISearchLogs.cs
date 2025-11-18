using static StoreServices.ClientsServices.SearchingLogsRepo;

namespace StoreServices.ClientsServices
{
    public interface ISearchLogs
    {
        Task AddSearchAsync(string SearchTerm, int ClientId);
        Task<List<string>> FindById(int ClientId);
        Task<Dictionary<string,int>> FindTheMostlyProductThatPeopleSearchedOn();
        Task<List<SearchLogWithClientName>> GetAll();

    }
}