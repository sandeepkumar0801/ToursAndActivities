using ActivityWrapper;
using Isango.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAPI.Filters;

namespace WebAPI.Controllers
{
    [Route("api/temp")]
    [ApiController]

    public class TempController : ApiBaseController
    {
        private readonly ActivityWrapperService _activityWrapperService;

        public TempController(ActivityWrapperService activityWrapperService)
        {
            _activityWrapperService = activityWrapperService;
        }

        [Route("performance")]
        [HttpGet]
        [ValidateModel]
        public IActionResult GetPerformanceResult(string activityIdsBlobURL)
        {
            var client = new HttpClient();
            if (string.IsNullOrWhiteSpace(activityIdsBlobURL))
            {
                activityIdsBlobURL = "https://bumblebeestorage.blob.core.windows.net/temp/activityIds.txt?st=2019-03-05T12%3A54%3A54Z&se=2019-10-01T12%3A54%3A00Z&sp=r&sv=2018-03-28&sr=b&sig=t%2FoonOBtDEPHz98Qf5QpoZyQftESEYcjnLwHIofz1f0%3D";
            }
            var result = client.GetStringAsync(activityIdsBlobURL).Result;
            var activityIds = result.Split(',').Select(Int32.Parse).ToList();
            var clientInfo = new ClientInfo
            {
                AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183",
                CountryIp = "gb",
                LanguageCode = "en"
            };
            var watch = new Stopwatch();
            watch.Reset();
            watch.Start();
            var activities = _activityWrapperService.GetActivities(activityIds, clientInfo);
            watch.Stop();
            var returnMsg = $" Time Taken to execute {activityIds.Count} is {watch.Elapsed}, Final Count {activities.Count()}";
            return GetResponseWithActionResult(returnMsg);
        }
    }
}