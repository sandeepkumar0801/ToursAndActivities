using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Entities;
using Util;

namespace ServiceAdapters.Rayna.Rayna.Commands
{
    public class RaynaTourOptionCmdHandler : CommandHandlerBase, IRaynaTourOptionCmdHandler
    {
        public RaynaTourOptionCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var data = criteria as TourOptionsRQ;
            return data;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(TourOptions);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<TourOptions>(responseText);
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
            var data = input as TourOptionsRQ;
            var url = string.Format($"{_raynaURL}{Constants.Constants.TourOptions}");
            var response =  GetResponseFromAPIEndPoint(data, url);
            return response;
        }
    }
}