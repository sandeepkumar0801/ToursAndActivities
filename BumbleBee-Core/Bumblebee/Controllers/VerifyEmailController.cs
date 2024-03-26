using Isango.Service.Contract;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Filters;

namespace WebAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [Route("api")]
    [ApiController]

    public class VerifyEmailController : ApiBaseController
    {
        private INeverBounceService _neverBounceService;
        //private INeverBounceAdapter _neverBounceAdapter;

        public VerifyEmailController(INeverBounceService neverBounceService)
        {
            _neverBounceService = neverBounceService;

        }

        /// <summary>
        /// This operation is used to verify neverBounce
        /// </summary>
        /// <param name="emailId"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        [Route("verifyemail/{emailId}/{tokenId}")]
        [HttpGet]
        [ValidateModel]
        public IActionResult VerifyEmail(string emailId, string tokenId)
        {
            var result = _neverBounceService.IsEmailVerified(emailId, tokenId);
            return GetResponseWithActionResult(result);
        }
    }
}