using DiscountRuleEngine.Contracts;
using DiscountRuleEngine.Model;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Filters;
using WebAPI.Mapper;
using WebAPI.Models.RequestModels;

namespace WebAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [Route("api/discount")]
    [ApiController]

    public class DiscountController : ApiBaseController
    {
        private readonly IDiscountEngine _discountEngine;
        private readonly DiscountMapper _discountMapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="discountEngine"></param>
        /// <param name="discountMapper"></param>
        public DiscountController(IDiscountEngine discountEngine, DiscountMapper discountMapper)
        {
            _discountEngine = discountEngine;
            _discountMapper = discountMapper;
        }

        /// <summary>
        /// This operation is used to apply discount on the cart
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("processdiscount")]
        [HttpPost]
        [ValidateModel]
        public IActionResult ProcessDiscount(ProcessDiscountRequest request)
        {
            // Preparing Discount Rule Engine request model
            var discountRequest = _discountMapper.MapDiscountRequest(request);

            if (discountRequest == null)
                return GetResponseWithActionResult((DiscountModel)null);
            // Discount Rule Engine call
            var discountCart = _discountEngine.Process(discountRequest);

            // Preparing API response from the Discount Cart
            var discountResponse = _discountMapper.MapDiscountResponse(discountCart);
            return GetResponseWithActionResult(discountResponse);
        }
    }
}