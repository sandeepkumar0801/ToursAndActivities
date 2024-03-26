using CacheManager.Contract;
using Isango.Entities;
using Isango.Persistence.Contract;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;

namespace Isango.Service
{
    public class LandingService : ILandingService
    {
        private readonly ILandingPersistence _landingPersistence;
        private readonly ILogger _log;

        public LandingService(ILandingCacheManager landingCacheManager, ILandingPersistence landingPersistence, ILogger log)
        {
            _landingPersistence = landingPersistence;
            _log = log;
        }

        /// <summary>
        /// This method return the list of the all the destination and all the attractions
        /// </summary>
        /// <returns></returns>
        public async Task<List<LocalizedMerchandising>> LoadLocalizedMerchandisingAsync()
        {
            try
            {
                var result = _landingPersistence.LoadLocalizedMerchandising();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "LandingService",
                    MethodName = "LoadLocalizedMerchandising"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of popular destinations
        /// </summary>
        /// <param name="sourceMarket"></param>
        /// <param name="language"></param>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        public async Task<List<LocalizedMerchandising>> GetPopularDestinationsListAsync(string sourceMarket, string language, string affiliateId)
        {
            try
            {
                var localizedMerchandising = await LoadLocalizedMerchandisingAsync();
                return localizedMerchandising?.FindAll(region => (region.Type.Equals(Constant.DestinationCode) && region.SourceMarket.Equals(sourceMarket) && region.Language.Equals(language) && region.AffiliateId.ToUpperInvariant().Equals(affiliateId)));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "LandingService",
                    MethodName = "GetPopularDestinationsList",
                    AffiliateId = affiliateId,
                    Params = $"{sourceMarket}{language}{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of popular Attractions
        /// </summary>
        /// <param name="sourceMarket"></param>
        /// <param name="language"></param>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        public async Task<List<LocalizedMerchandising>> GetPopularAttractionListAsync(string sourceMarket, string language, string affiliateId)
        {
            try
            {
                var localizedMerchandising = await LoadLocalizedMerchandisingAsync();
                return localizedMerchandising?.FindAll(region => (region.Type.Equals(Constant.AttractionCode) && region.SourceMarket.Equals(sourceMarket) && region.Language.Equals(language) && region.AffiliateId.ToUpperInvariant().Equals(affiliateId)));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "LandingService",
                    MethodName = "GetPopularAttractionList",
                    AffiliateId = affiliateId,
                    Params = $"{sourceMarket}{language}{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}