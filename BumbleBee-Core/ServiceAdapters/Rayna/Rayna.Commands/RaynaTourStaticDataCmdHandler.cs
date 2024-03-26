using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Entities;
using Util;

namespace ServiceAdapters.Rayna.Rayna.Commands
{
    public class RaynaTourStaticDataCmdHandler : CommandHandlerBase, IRaynaTourStaticDataCmdHandler
    {
        public RaynaTourStaticDataCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var data = criteria as TourStaticDataRQ;
            return data;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(TourStaticData);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<TourStaticData>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }

        protected override object GetResults(object input)
        {
            var data = input as TourStaticDataRQ;
            var datapas = new TourStaticDataRQ
            {
                CountryId = data.CountryId,
                CityId= data.CityId
            };
            var url = string.Format($"{_raynaURL}{Constants.Constants.TourStaticData}");
            var response =  GetResponseFromAPIEndPoint(datapas, url);
            return response;
        }
    }
}