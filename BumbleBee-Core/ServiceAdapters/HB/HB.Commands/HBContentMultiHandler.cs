using Logger.Contract;
using ServiceAdapters.HB.Constants;
using ServiceAdapters.HB.HB.Commands.Contract;
using ServiceAdapters.HB.HB.Entities.ContentMulti;
using System;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.HB.HB.Commands
{
    public class HBContentMultiCmdHandler : CommandHandlerBase, IHBContentMultiCmdHandler
    {
        public HBContentMultiCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var hotelbedCriteriaApitude = criteria as ContentMultiRq;

            return hotelbedCriteriaApitude;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(ContentMultiRS);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<ContentMultiRS>(responseText);
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
            var url = string.Format($"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.HBApitudeContentServiceURL)}{Constant.ContentMultiURL}");
            var response = await GetResponseFromAPIEndPoint(input, url);
            return response;
        }
    }
}