using Isango.Entities;
using Isango.Service.Constants;
using Microsoft.AspNetCore.Mvc;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using WebAPI.Filters;
using WebAPI.Helper;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;
using ILogger = Logger.Contract.ILogger;
using PaymentExtraDetail = WebAPI.Models.ResponseModels.PaymentExtraDetail;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [CustomActionWebApiFilter]
    [ApiController]
    public class CheckoutController : ApiBaseController
    {
        private readonly ITableStorageOperation _tableStorageOperation;
        private readonly CheckoutHelper _checkoutHelper;
        private readonly ILogger _log;
        /// <summary>
        ///  Parameterized Constructor to initialize all dependencies.
        /// </summary>
        /// <param name="tableStorageOperation"></param>
        /// <param name="checkoutHelper"></param>
        /// <param name="log"></param>
        public CheckoutController(ITableStorageOperation tableStorageOperation,
            CheckoutHelper checkoutHelper, ILogger log = null)
        {
            _tableStorageOperation = tableStorageOperation;
            _checkoutHelper = checkoutHelper;
            _log = log;
        }
        /// <summary>
        /// This operation is used to get extra details required for products on confirm booking
        /// </summary>
        /// <param name="paymentExtraDetailsRequest"></param>
        /// <returns></returns>
        //[Route("{v1?}/getpaymentextradetails")]
        [Route("getpaymentextradetails")]
        [HttpPost]
        [ValidateModel]
        //[SwitchableAuthorization]
        public IActionResult GetPaymentExtraDetails(PaymentExtraDetailsRequest paymentExtraDetailsRequest)
        {
            //var identity = User.Identity as ClaimsIdentity;
            //if (string.IsNullOrWhiteSpace(paymentExtraDetailsRequest.TokenId))
            //{
            //    if (identity != null)
            //    {
            //        paymentExtraDetailsRequest.TokenId = identity?.FindFirst("affiliateId")?.Value;
            //    }
            //}
            var extraDetailsResponse = new List<PaymentExtraDetail>();
            foreach (var extraDetailsRequest in paymentExtraDetailsRequest.PaymentExtraDetails)
            {
                var baseAvailabilitiesEntity = _tableStorageOperation.RetrieveData<BaseAvailabilitiesEntity>(extraDetailsRequest.ReferenceId, paymentExtraDetailsRequest.TokenId);
                if (baseAvailabilitiesEntity != null)
                {
                    _checkoutHelper.GetPaymentExtraDetails(extraDetailsRequest, baseAvailabilitiesEntity, paymentExtraDetailsRequest.TokenId, ref extraDetailsResponse);
                }
                else
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "CheckoutController",
                        MethodName = "GetPaymentExtraDetails",
                        Token = paymentExtraDetailsRequest?.TokenId,
                        Params = $"{extraDetailsRequest.ReferenceId}"
                    };
                    string message = Constant.DataNotInTableStorage + " Token:" + paymentExtraDetailsRequest.TokenId;
                    _log.Error(isangoErrorEntity, new Exception(message));
                    return NotFound(message);
                }
            }
            var response = new PaymentExtraDetailsResponse { PaymentExtraDetails = extraDetailsResponse, TokenId = paymentExtraDetailsRequest.TokenId };
            return GetResponseWithActionResult(response);
        }
        /// <summary>
        /// This Operation is used to get the Club Vistara membership number 
        /// </summary>
        /// <param name="inBondSerialNumber"></param>
        /// <returns></returns>
        [Route("GetFFPAccountnumber/{inBondSerialNumber}")]
        [HttpGet]
        public IActionResult GetFFPAccountnumber(int inBondSerialNumber)
        {
            int remainder = inBondSerialNumber % 7;
            int nineDigitSerialNumber = (inBondSerialNumber * 10) + remainder;
            return GetResponseWithActionResult(nineDigitSerialNumber);
        }
    }
}
