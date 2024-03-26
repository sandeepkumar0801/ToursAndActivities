using Logger.Contract;
using ServiceAdapters.HB.Constants;
using ServiceAdapters.HB.HB.Commands.Contract;
using ServiceAdapters.HB.HB.Entities.Calendar;
using System;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.HB.HB.Commands
{
    public class HBCalendarCmdHandler : CommandHandlerBase, IHBCalendarCmdHandler
    {
        public HBCalendarCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var hotelbedCriteriaApitude = criteria as Isango.Entities.HotelBeds.HotelbedCriteriaApitudeFilter;
            var activityRq = GetCriteriaCalendarRequest(hotelbedCriteriaApitude);
            return activityRq;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(CalendarRs);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<CalendarRs>(responseText);
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
            var url = string.Format($"{_hotelBedsApitudeServiceURL}{Constant.CalendarURL}");
            var response = await GetResponseFromAPIEndPoint(input, url);
            return response;
        }
    }
}