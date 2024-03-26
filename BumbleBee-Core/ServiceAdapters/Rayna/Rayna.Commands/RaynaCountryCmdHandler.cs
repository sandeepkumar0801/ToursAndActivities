using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Entities;
using Util;

namespace ServiceAdapters.Rayna.Rayna.Commands
{
    public class RaynaCountryCmdHandler : CommandHandlerBase, IRaynaCountryCmdHandler
    {
        public RaynaCountryCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            return criteria;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(Countries);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<Countries>(responseText);
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
            var url = string.Format($"{_raynaURL}{Constants.Constants.Countries}");
            
            var response =  GetResponseFromAPIEndPoint(input, url,"GET");
            return response;
        }
    }
}