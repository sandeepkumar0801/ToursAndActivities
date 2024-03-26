using Isango.Service.Contract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Util;
using WebAPI.Helper;
using WebAPI.Mapper;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Provide details of all Master service related endpoints
    /// </summary>
    [Route("api")]
    [ApiController]

    public class MasterController : ApiBaseController
    {
        private readonly IMasterService _masterService;
        private readonly MasterMapper _masterMapper;
        private readonly MasterHelper _masterHelper;




        /// <summary>
        ///Parameterized Constructor to initialize all dependencies.
        /// </summary>
        /// <param name="masterService"></param>
        /// <param name="masterMapper"></param>
        public MasterController(IMasterService masterService,
            MasterMapper masterMapper, MasterHelper masterHelper
            )
        {
            _masterService = masterService;
            _masterMapper = masterMapper;
            _masterHelper = masterHelper;
        }


        /// <summary>
        /// This method returns currency exchange rate.
        /// </summary>
        /// <param name="fromCurrency"></param>
        /// <param name="toCurrency"></param>
        /// <returns></returns>
        [Route("master/conversionfactor/{fromCurrency}/{toCurrency}")]
        //  [SwitchableAuthorization]
        [HttpGet]
        public IActionResult GetConversionFactor(string fromCurrency, string toCurrency)
        {
            var exchangeRate = _masterService.GetConversionFactorAsync(fromCurrency, toCurrency).Result;

            var exchangeRateResponse = new CurrencyExchangeRateResponse()
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                ExchangeRateValue = exchangeRate
            };

            return GetResponseWithActionResult(exchangeRateResponse);
        }

        /// <summary>
        /// This method returns currency master data.
        /// </summary>
        /// <returns></returns>
        [Route("master/currency")]
        //[SwitchableAuthorization]
        [HttpGet]

        public IActionResult GetCurrency()
        {
            var masterCurrency = _masterService.GetCurrencyAsync().Result;
            var masterResponse = _masterMapper.MapCurrencyMasterRequest(masterCurrency);
            return GetResponseWithActionResult(masterResponse);
        }

        /// <summary>
        /// This method returns languages master data.
        /// </summary>
        /// <returns></returns>
        [Route("master/language")]
        //[SwitchableAuthorization]
        [HttpGet]

        public IActionResult GetLanguages()
        {
            var masterLanguages = _masterService.GetMasterLanguagesAsync().Result;
            var masterResponse = _masterMapper.MapLanguageMasterRequest(masterLanguages);
            return GetResponseWithActionResult(masterResponse);
        }

        /// <summary>
        /// This method returns geo detail master data.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        [Route("master/geodetail/{language}")]
        //[DefaultValue("en")]
        // [SwitchableAuthorization]
        [HttpGet]

        public IActionResult GetGeoDetails(string language = null)
        {
            var masterGeoDetails = _masterService.GetMasterGeoDetailAsync(language).Result;
            var masterResponse = _masterMapper.MapGeoDetailsMasterRequest(masterGeoDetails);
            return GetResponseWithActionResult(masterResponse);
        }

        /// <summary>
        /// This method returns region wise master data.
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        [Route("master/activity/{affiliateId}")]
        //[SwitchableAuthorization]
        [HttpGet]

        public IActionResult GetRegionWiseLiveIsangoActivities(string affiliateId)
        {
            var masterRegionWise = _masterService.GetMasterRegionWiseAsync(affiliateId).Result;
            var masterResponse = _masterMapper.MapRegionWiseMasterRequest(masterRegionWise);

            return GetResponseWithActionResult(masterResponse);
        }

        /// <summary>
        /// This method returns adyen payment methods data.
        /// </summary>
        /// <returns></returns>
        [Route("master/adyen/paymentmethods")]
        //  [SwitchableAuthorization]
        [HttpGet]
        public IActionResult GetAdyenPaymentMethods(string countryCode
            , string shopperLocale, string amount, string currency)
        {
            var paymentMethods = _masterService.GetAdyenPaymentMethodsAsync(countryCode
            , shopperLocale, amount, currency).Result;
            return GetResponseWithActionResult(paymentMethods);
        }

        /// <summary>
        /// This method returns adyen payment methods data.
        /// </summary>
        /// <returns></returns>
        [Route("master/adyen/paymentmethodsV2")]
        //  [SwitchableAuthorization]
        [HttpGet]
        public IActionResult GetAdyenPaymentMethodsV2(string countryCode
            , string shopperLocale, string amount, string currency)
        {
            var paymentMethodsJson = _masterService.GetAdyenPaymentMethodsV2Async(countryCode
            , shopperLocale, amount, currency).Result;
            return GetResponseWithActionResult(paymentMethodsJson);
        }

        [Route("master/PushActivityToCache/{activityIds}")]
        //[SwitchableAuthorization]
        [HttpGet]
        public IActionResult LoadSelectedActivitiesAsync(string activityIds)
        {
            var paymentMethodsJson = _masterHelper.LoadSelectedActivities(activityIds).Result;
            return GetResponseWithActionResult(paymentMethodsJson);
        }

        /// <summary>
        /// This method saves Image Alt Tags in DB.
        /// </summary>
        /// <returns></returns>
        [Route("master/image/alttag")]
        [HttpPost]
        public async Task<IActionResult> ProcessImageAltTag()
        {
            using (var streamReader = new StreamReader(Request.Body))
            {
                var jsonPostedData = await streamReader.ReadToEndAsync();
                var notification = JsonConvert.DeserializeObject<CloudinaryNotification>(jsonPostedData);
                if (notification != null && (notification.resources?.Count ?? 0) > 0)
                {
                    foreach (var resource in notification.resources)
                    {
                        var altText = string.Empty;
                        var imageName = resource.Key;
                        if (resource.Value?.added != null && resource.Value.added.Count() > 0 && resource.Value.added[0].name.ToLower().Contains("alt"))
                        {
                            altText = resource.Value.added[0].value;
                        }
                        if (resource.Value?.updated != null && resource.Value.updated.Count() > 0 && resource.Value.updated[0].name.ToLower().Contains("alt"))
                        {
                            altText = resource.Value.updated[0].value;
                        }
                        if (!string.IsNullOrEmpty(altText))
                        {
                            _masterService.SaveImageAltText(imageName, altText);
                        }
                    }
                }
            }
            return Ok();
        }
        private string GetAffiliateFromIdentity()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                var affiliateId = userClaims.FirstOrDefault(o => o.Type == "affiliateId")?.Value;
                return affiliateId;
            }
            return null;
        }
        /// <summary>
        /// This method returns region wise master data.
        /// </summary>
        /// <param name="activityLiteCriteria"></param>
        /// <returns></returns>
        [Route("ActivityLite/v1/activity")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [SwaggerOperation(Tags = new[] { "Activity Lite" })]
        //[SwaggerResponse(HttpStatusCode.OK, "Region type Response Model", typeof(RegionWiseMasterResponse))]
        public IActionResult GetRegionWiseLiveIsangoActivities_ActivityLite(ActivityLiteCriteria activityLiteCriteria)
        {

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var affiliateId = activityLiteCriteria.AffiliateId;
            var categoryid = activityLiteCriteria.CategoryIds;
            if (string.IsNullOrWhiteSpace(affiliateId))
            {
                if (identity != null)
                {
                    var userClaims = identity.Claims;
                    affiliateId = userClaims.FirstOrDefault(o => o.Type == "affiliateId")?.Value;
                    activityLiteCriteria.AffiliateId = affiliateId;

                }
            }
            if (string.IsNullOrEmpty(activityLiteCriteria.TokenId))
            {
                activityLiteCriteria.TokenId = affiliateId;
            }
            var masterRegionWise = _masterService.GetMasterRegionWiseAsync(affiliateId, categoryid, "true").Result;

            if (masterRegionWise == null)
            {
                return NotFound();
            }

            var masterResponse = _masterMapper.MapRegionWiseMasterRequest(masterRegionWise);
            return GetResponseWithActionResult(masterResponse);
        }

        [HttpGet]
        [Route("GetConfig")]
        public IActionResult GetConfig()
        {
            var envValue = ConfigurationManagerHelper.GetValuefromAppSettings("Environment"); // Replace with your actual key
            return Ok(new { EnvValue = envValue });
        }

    }
}