//using Isango.Service.Contract;
//using Microsoft.AspNetCore.Mvc;
//using WebAPI.Filters;
//using WebAPI.Mapper;

//namespace WebAPI.Controllers
//{
//    /// <summary>
//    /// Provide details of all delta service related endpoints
//    /// </summary>

//    [Route("api/delta")]
//    [ApiController]

//    public class DeltaMasterController : ApiBaseController
//    {
//        /* No longer used
//        private readonly IMasterService _masterService;
//        private readonly MasterMapper _masterMapper;

//        /// <summary>
//        ///Parameterized Constructor to initialize all dependencies.
//        /// </summary>
//        /// <param name="masterService"></param>
//        /// <param name="masterMapper"></param>
//        public DeltaMasterController(IMasterService masterService, MasterMapper masterMapper)
//        {
//            _masterService = masterService;
//            _masterMapper = masterMapper;
//        }

//        /// <summary>
//        /// This method returns Affiliate.
//        /// </summary>
//        /// <returns></returns>
//        [SwaggerOperation(Tags = new[] { "Delta Master" })]
//        [Route("affiliate")]
//        [SwitchableAuthorization]
//        public IHttpActionResult GetAffiliate()
//        {
//            var affiliate = _masterService.GetDeltaAffiliateAsync().Result;
//            var affiliateResponse = _masterMapper.MapAffiliateRequest(affiliate);
//            return GetResponseWithHttpActionResult(affiliateResponse);
//        }

//        /// <summary>
//        /// This method returns Region Attraction.
//        /// </summary>
//        /// <returns></returns>
//        [SwaggerOperation(Tags = new[] { "Delta Master" })]
//        [Route("attraction")]
//        [SwitchableAuthorization]
//        public IHttpActionResult GetAttraction()
//        {
//            var localizedMerchandising = _masterService.GetDeltaAttractionsAsync().Result;
//            var localizedMerchandisingResponse = _masterMapper.MapAttractionRequest(localizedMerchandising);
//            return GetResponseWithHttpActionResult(localizedMerchandisingResponse);
//        }

//        /// <summary>
//        /// This method returns Region Attraction.
//        /// </summary>
//        /// <returns></returns>
//        [SwaggerOperation(Tags = new[] { "Delta Master" })]
//        [Route("regionattraction")]
//        [SwitchableAuthorization]
//        public IHttpActionResult GetRegionAttraction()
//        {
//            var regionAttraction = _masterService.GetDeltaRegionAttractionAsync().Result;
//            var regionAttractionResponse = _masterMapper.MapRegionAttractionRequest(regionAttraction);
//            return GetResponseWithHttpActionResult(regionAttractionResponse);
//        }

//        /// <summary>
//        /// This method returns Region Attraction.
//        /// </summary>
//        /// <returns></returns>
//        [SwaggerOperation(Tags = new[] { "Delta Master" })]
//        [Route("regionsubattraction")]
//        [SwitchableAuthorization]
//        public IHttpActionResult GetRegionSubAttraction()
//        {
//            var regionSubAttraction = _masterService.GetDeltaRegionSubAttractionAsync().Result;
//            var regionSubAttractionResponse = _masterMapper.MapRegionSubAttractionRequest(regionSubAttraction);
//            return GetResponseWithHttpActionResult(regionSubAttractionResponse);
//        }

//        /// <summary>
//        /// This method returns Geo Details.
//        /// </summary>
//        /// <returns></returns>
//        [SwaggerOperation(Tags = new[] { "Delta Master" })]
//        [Route("geodetail")]
//        [SwitchableAuthorization]
//        public IHttpActionResult GetGeoDetails()
//        {
//            var geoDetails = _masterService.GetGeoDetailAsync().Result;
//            var geoDetailsResponse = _masterMapper.MapGeoDetailsRequest(geoDetails);
//            return GetResponseWithHttpActionResult(geoDetailsResponse);
//        }

//        /// <summary>
//        /// This method returns Geo Details.
//        /// </summary>
//        /// <returns></returns>
//        [SwaggerOperation(Tags = new[] { "Delta Master" })]
//        [Route("destination")]
//        [SwitchableAuthorization]
//        public IHttpActionResult GetDestinations()
//        {
//            var destination = _masterService.GetDestinationAsync().Result;
//            var destinationResponse = _masterMapper.MapDestinationRequest(destination);
//            return GetResponseWithHttpActionResult(destinationResponse);
//        }

//        /// <summary>
//        /// This method returns Product Supplier.
//        /// </summary>
//        /// <returns></returns>
//        [SwaggerOperation(Tags = new[] { "Delta Master" })]
//        [Route("productsupplier")]
//        public IHttpActionResult GetProductSupplier()
//        {
//            var productSupplier = _masterService.GetProductSupplierAsync().Result;
//            var productSupplierResponse = _masterMapper.MapProductSupplierRequest(productSupplier);
//            return GetResponseWithHttpActionResult(productSupplierResponse);
//        }

//        /////// <summary>
//        /////// This method returns Region Attraction.
//        /////// </summary>
//        /////// <returns></returns>
//        //[SwaggerOperation(Tags = new[] { "Delta Master" })]
//        //[Route("destination")]
//        //public IHttpActionResult GetDestinations(string languageCode = "en")
//        //{
//        //    var destination = _masterService.GetDestinationAsync(languageCode).Result;
//        //    var destinationResponse = _masterMapper.MapDestinationRequest(destination);
//        //    return GetResponseWithHttpActionResult(destinationResponse);
//        //}

//        ///// <summary>
//        ///// This method returns badge.
//        ///// </summary>
//        ///// <returns></returns>
//        //[SwaggerOperation(Tags = new[] { "Delta Master" })]
//        //[Route("badges")]
//        //public IHttpActionResult GetBadge()
//        //{
//        //    var badge = _masterService.GetBadgeAsync().Result;
//        //    var badgeResponse = _masterMapper.MapBadgeRequest(badge);
//        //    return GetResponseWithHttpActionResult(badgeResponse);
//        //}

//        ///// <summary>
//        ///// This method returns currency.
//        ///// </summary>
//        ///// <returns></returns>
//        //[SwaggerOperation(Tags = new[] { "Delta Master" })]
//        //[Route("currency")]
//        //public IHttpActionResult GetCurrency()
//        //{
//        //    var currency = _masterService.GetCurrencyAsync().Result;
//        //    var currencyResponse = _masterMapper.MapCurrencyRequest(currency);
//        //    return GetResponseWithHttpActionResult(currencyResponse);
//        //}

//        ///// <summary>
//        ///// This method returns language.
//        ///// </summary>
//        ///// <returns></returns>
//        //[SwaggerOperation(Tags = new[] { "Delta Master" })]
//        //[Route("languages")]
//        //public IHttpActionResult GetLanguage()
//        //{
//        //    var language = _masterService.GetLanguageAsync().Result;
//        //    var languageResponse = _masterMapper.MapLanguageRequest(language);
//        //    return GetResponseWithHttpActionResult(languageResponse);
//        //}

//        ///// <summary>
//        ///// This method returns Exchange Rate.
//        ///// </summary>
//        ///// <returns></returns>
//        //[SwaggerOperation(Tags = new[] { "Delta Master" })]
//        //[Route("exchangerate")]
//        //public IHttpActionResult GetExchangeRate()
//        //{
//        //    var exchangeRate = _masterService.LoadCurrencyExchangeRatesAsync().Result;
//        //    var exchangeRateResponse = _masterMapper.MapExchangeRateRequest(exchangeRate);
//        //    return GetResponseWithHttpActionResult(exchangeRateResponse);
//        //}
//        */
//    }
//}