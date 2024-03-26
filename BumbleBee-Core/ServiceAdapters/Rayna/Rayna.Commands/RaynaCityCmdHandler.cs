using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Entities;
using Util;

namespace ServiceAdapters.Rayna.Rayna.Commands
{
    public class RaynaCityCmdHandler : CommandHandlerBase, IRaynaCityCmdHandler
    {
        public RaynaCityCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var data = criteria as CityByCountryRQ;
            return data;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(CityByCountry);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<CityByCountry>(responseText);
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
            var data = input as CityByCountryRQ;
            var datapas = new CityByCountryRQ
            {
                CountryId = data.CountryId
            };
            var url = string.Format($"{_raynaURL}{Constants.Constants.Cities}");
            var response =  GetResponseFromAPIEndPoint(datapas, url);
            return response;
        }
    }
}