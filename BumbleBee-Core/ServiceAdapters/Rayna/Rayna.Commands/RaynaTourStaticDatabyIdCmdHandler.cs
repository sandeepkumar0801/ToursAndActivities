using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Entities;
using Util;

namespace ServiceAdapters.Rayna.Rayna.Commands
{
    public class RaynaTourStaticDatabyIdCmdHandler : CommandHandlerBase, IRaynaTourStaticDataByIdCmdHandler
    {
        public RaynaTourStaticDatabyIdCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var data = criteria as TourStaticDataByIdRQ;
            return data;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(TourStaticDataById);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<TourStaticDataById>(responseText);
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
            var data = input as TourStaticDataByIdRQ;
            var datapas = new TourStaticDataByIdRQ
            {
                CountryId = data.CountryId,
                CityId= data.CityId,
                ContractId= data.ContractId,
                TourId= data.TourId,
                TravelDate= data.TravelDate
            };
            var url = string.Format($"{_raynaURL}{Constants.Constants.TourStaticDataById}");
            var response =  GetResponseFromAPIEndPoint(datapas, url);
            return response;
        }
    }
}