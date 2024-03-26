using Isango.Service.Contract;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebAPI.Mapper;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MarketingController : ControllerBase
    {
        private readonly IMasterService _masterService;
        private readonly MasterMapper _masterMapper;

        public MarketingController(IMasterService masterService, MasterMapper masterMapper)
        {
            _masterService = masterService;
            _masterMapper = masterMapper;
        }

        [HttpGet("cjfeed/{currency}")]
        public async Task<IActionResult> GetCJFeed(string currency)
        {
            int cid;
            switch (currency.ToUpperInvariant())
            {
                case "EUR":
                    cid = 2;
                    break;
                case "USD":
                    cid = 3;
                    break;
                default:
                    cid = 4;
                    break;
            }

            var cjFeed = await _masterService.GetMarketingCJFeedAsync(cid);
            var cjFeedResponse = _masterMapper.MapCjFeedRequest(cjFeed, currency.ToUpperInvariant());

            return Content(cjFeedResponse, "text/xml", Encoding.UTF8);
        }

        [HttpGet("criteofeed/{currency}")]
        //[SwaggerOperation(Tags = new[] { "Marketing" })]
        public async Task<IActionResult> GetCriteoFeed(string currency)
        {
            var criteoFeed = await _masterService.GetMarketingCriteoFeedAsync(currency);
            var criteoFeedResponse = _masterMapper.MapCJFeedProductRequest(criteoFeed);

            return Ok(criteoFeedResponse);
        }
    }
}
