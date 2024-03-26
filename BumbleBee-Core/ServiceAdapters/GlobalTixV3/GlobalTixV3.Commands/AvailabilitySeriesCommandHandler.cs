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
    public class AvailabilitySeriesCommandHandler : CommandHandlerBase, IAvailabilitySeriesCommandHandler
    {
        #region Constructors
        public AvailabilitySeriesCommandHandler(ILogger iLog) : base(iLog)
        {
        }
        #endregion

        protected override object CreateInputRequest(InputContext inputContext)
        {
            InputContext availCtx = new InputContext();
            if (availCtx == null)
            {
                return null;
            }

            IDictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add(Constant.QueryParam_TicketTypeID, ((ActivityInfoInputContext)inputContext).TicketType.ToString());
            queryParams.Add(Constant.QueryParam_DateFrom, ((ActivityInfoInputContext)inputContext).CheckinDate.ToString("yyyy-MM-dd"));
            queryParams.Add(Constant.QueryParam_DateTo, ((ActivityInfoInputContext)inputContext).CheckOutDate.ToString("yyyy-MM-dd"));
            return queryParams;
        }

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }

        protected override object GetResults(object input, string authString, bool isNonThailandProduct)
        {
            IDictionary<string, string> queryParams = input as IDictionary<string, string>;
            if (queryParams == null)
            {
                return null;
            }

            AsyncClient client = GetServiceClient(Constant.URL_CheckEventAvailability);
            return client.ConsumeGetServicePrioHub(GetHttpRequestHeaders(isNonThailandProduct), queryParams);
        }

        protected override Task<object> GetResultsAsync(object input, string authString, bool isNonThailandProduct)
        {
            if (input == null) return null;

            var client = GetServiceClient(Constant.URL_CheckEventAvailability);
            IDictionary<string, string> queryParams = new Dictionary<string, string>();

            return client.ConsumeGetServiceAsync(GetHttpRequestHeaders(isNonThailandProduct), queryParams);
        }
    }
}
