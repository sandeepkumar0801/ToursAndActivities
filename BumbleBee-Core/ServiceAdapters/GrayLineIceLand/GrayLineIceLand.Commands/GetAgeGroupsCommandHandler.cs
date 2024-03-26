using Logger.Contract;
using ServiceAdapters.GrayLineIceLand.Constants;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands.Contracts;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels;
using System;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands
{
    public class GetAgeGroupsCommandHandler : CommandHandlerBase, IGetAgeGroupsCommandHandler
    {
        public GetAgeGroupsCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override async Task<string> GetStringResultsAsync(object input)
        {
            if (input != null)
            {
                var inputContext = input as InputContext;
                var client = new AsyncClient
                {
                    ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineBaseUrl)}{Constant.AgeGroupApiUrl}{inputContext?.TourNumber}"
                };
                var jsonResponse = await client.SendGetRequest(String.Empty, String.Empty);
                return jsonResponse;
            }
            return null;
        }

        protected override async Task<object> GetResultsAsync(object input, string authString)
        {
            return null;
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            return inputContext;
        }

        protected override object GetResults(object input, string authString)
        {
            return null;
        }

        protected override object GetResults(object input, string authString, out string requestXml, out string responseXml)
        {
            requestXml = "";
            responseXml = "";
            return null;
        }
    }
}