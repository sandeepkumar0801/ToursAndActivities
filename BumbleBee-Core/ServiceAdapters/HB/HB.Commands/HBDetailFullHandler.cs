using Logger.Contract;
using ServiceAdapters.HB.Constants;
using ServiceAdapters.HB.HB.Commands.Contract;
using System;
using System.Threading.Tasks;
using Util;
using DetailFull = ServiceAdapters.HB.HB.Entities.ActivityDetailFull;

namespace ServiceAdapters.HB.HB.Commands
{
    public class HBDetailFullCmdHandler : CommandHandlerBase, IHbDetailFullCmdHandler
    {
        public HBDetailFullCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var hotelbedCriteriaApitude = criteria as Isango.Entities.HotelBeds.HotelbedCriteriaApitude;
            var activityRq = GetActivityDetailReqeust(hotelbedCriteriaApitude);
            return activityRq;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(DetailFull.ActivityDetailFullRS);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<DetailFull.ActivityDetailFullRS>(responseText);
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