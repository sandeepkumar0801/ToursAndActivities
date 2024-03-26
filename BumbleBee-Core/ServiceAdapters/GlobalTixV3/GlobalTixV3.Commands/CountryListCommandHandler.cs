using ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger.Contract;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ServiceAdapters.GlobalTixV3.Constants;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands
{
    public class CountryListCommandHandler : CommandHandlerBase, ICountryListCommandHandler
    {
        #region Class Constructors
        public CountryListCommandHandler(ILogger iLog) : base(iLog)
        {
        }
        #endregion

        protected override object CreateInputRequest(InputContext inputContext)
        {
            return null;
        }

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }

        protected override object GetResults(object input, string token,bool isNonThailandProduct)
        {
            AsyncClient client = GetServiceClient(Constant.URL_CountryList);
            return client.ConsumeGetServicePrioHub(GetHttpRequestHeaders(isNonThailandProduct), null);
        }

        protected override Task<object> GetResultsAsync(object input, string authString,bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }
    }
}
