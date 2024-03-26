using Isango.Entities.HotelBeds;
using Logger.Contract;
using ServiceAdapters.HB.Constants;
using ServiceAdapters.HB.HB.Commands.Contract;
using System;
using System.Threading.Tasks;
using Util;
using ActivitySimple = ServiceAdapters.HB.HB.Entities.ActivityDetail;

namespace ServiceAdapters.HB.HB.Commands
{
    public class HBDeatilCmdHandler : CommandHandlerBase, IHbDeatilCmdHandler
    {
        public HBDeatilCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var hotelbedCriteriaApitude = criteria as HotelbedCriteriaApitude;
            var activityRq = GetActivityDetailReqeust(hotelbedCriteriaApitude);
            return activityRq;
        }

        /// <summary>
        /// Get Api response object to be passed to converter or dumping.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="responseText"></param>
        /// <returns></returns>

        protected override object GetResponseObject(string responseText)
        {
            var result = default(ActivitySimple.ActivityDetailRS);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<ActivitySimple.ActivityDetailRS>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }

        protected override async Task<object> GetResultsAsync(object input)
        {
            var url = string.Format($"{_hotelBedsApitudeServiceURL}{Constant.ActivitiesDetailsURL}");
            var response = await GetResponseFromAPIEndPoint(input, url);
            return response;
        }
    }
}