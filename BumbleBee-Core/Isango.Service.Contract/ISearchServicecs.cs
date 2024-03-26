using Isango.Entities;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface ISearchService
    {
        Task<SearchStack> GetSearchDataAsync(SearchCriteria searchCriteria, ClientInfo clientInfo, Criteria criteria);

        Task<SearchStack> GetSearchDataV2Async(SearchCriteria searchCriteria, ClientInfo clientInfo, Criteria criteria);

        SearchStack GetSearchStackWithActivityData(SearchCriteria searchCriteria, ClientInfo clientInfo);
    }
}