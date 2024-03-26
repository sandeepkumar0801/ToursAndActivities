using Isango.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface ILandingService
    {
        Task<List<LocalizedMerchandising>> LoadLocalizedMerchandisingAsync();

        Task<List<LocalizedMerchandising>> GetPopularDestinationsListAsync(string sourceMarket, string language, string affiliateId);

        Task<List<LocalizedMerchandising>> GetPopularAttractionListAsync(string sourceMarket, string language, string affiliateId);
    }
}