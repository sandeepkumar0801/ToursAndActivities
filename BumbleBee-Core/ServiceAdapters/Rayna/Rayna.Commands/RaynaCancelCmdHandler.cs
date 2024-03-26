using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Entities;
using Util;

namespace ServiceAdapters.Rayna.Rayna.Commands
{
    public class RaynaCancelCmdHandler : CommandHandlerBase, IRaynaCancelCmdHandler
    {
        public RaynaCancelCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T cancelREQ)
        {

            var returnData = cancelREQ as CancelREQ;
            return returnData;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(CancelRES);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<CancelRES>(responseText);
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
            var data = input as CancelREQ;
            var url = string.Format($"{_raynaURL}{Constants.Constants.Cancel}");
            var response = GetResponseFromAPIEndPoint(data, url);
            return response;
        }
    }
}