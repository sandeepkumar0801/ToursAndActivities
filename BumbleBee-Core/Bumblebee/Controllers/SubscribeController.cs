using Isango.Service.Contract;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPI.Filters;
using WebAPI.Mapper;
using WebAPI.Models.RequestModels;

namespace WebAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [Route("api")]
    [ApiController]

    public class SubscribeController : ApiBaseController
    {
        private readonly IManageIdentityService _manageIdentityService;
        private readonly SubscribeMapper _subscribeMapper;

        /// <summary>
        ///
        /// </summary>
        /// <param name="manageIdentityService"></param>
        /// <param name="subscribeMapper"></param>
        public SubscribeController(IManageIdentityService manageIdentityService, SubscribeMapper subscribeMapper)
        {
            _manageIdentityService = manageIdentityService;
            _subscribeMapper = subscribeMapper;
        }

        /// <summary>
        /// This operation is used to create newsletter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SubscribeNewsLetter")]
        [HttpPost]
        [ValidateModel]
        //public IHttpActionResult SubscribeNewsLetter(NewsLetter model)
        //{
        //    var criteria = _subscribeMapper.MapNewsLetterRequest(model);
        //    var newsLetterResponseModel = _subscribeMapper.MapNewsLetterResponse(_manageIdentityService.SubscribeNewsLetterAsync(criteria));
        //    return GetResponseWithHttpActionResult(newsLetterResponseModel);
        //}

        public IActionResult SubscribeNewsLetter(NewsLetter model)
        {
            if (!string.IsNullOrEmpty(model.EmailId) && !string.IsNullOrEmpty(model.Name)
                 && !string.IsNullOrEmpty(model.LanguageCode) && !string.IsNullOrEmpty(model.AffiliateId)
                 && !string.IsNullOrEmpty(model.CustomerOrigin))
            {
                var criteria = _subscribeMapper.MapNewsLetterRequest(model);
                string result = _subscribeMapper.MapNewsLetterResponse(_manageIdentityService.SubscribeNewsLetterAsync(criteria));
                return GetResponseWithActionResult(result);
            }
            else
            {
                string dataObject = null; // Your data object or null if not available
                string errorMessage = "Please enter required values."; // Your error message
                var statusCode = HttpStatusCode.BadRequest; // The desired HTTP status code

                return GetResponseWithActionResult<string>(dataObject, errorMessage, statusCode);
            }
        }

    }
}