using Logger.Contract;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceAdapters.GlobalTixV3.Constants;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels;
using Util;
using System.Text;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands
{
    public class CancelReleaseBookingCommandHandler : CommandHandlerBase, ICancelReleaseCommandHandler
    {
        public CancelReleaseBookingCommandHandler(ILogger iLog) : base(iLog)
        {

        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var cancelCtx = inputContext as BookInputContextV3;
            return
                (cancelCtx != null)
                    ? new Dictionary<string, string>() { { Constant.QueryParam_RefernceNumber, cancelCtx.BookingReferenceNumber } }
                    : null;

            //var cancelCtx = inputContext as CancelInputContext;
            //if (cancelCtx == null)
            //{
            //    return null;
            //}

            //var bookRQ = new CancelReleaseRequest() { ReferenceNumber = cancelCtx.BookingReference };
            //return bookRQ;
        }

        protected override object GetResults(object input, string authString, bool isNonThailandProduct)
        {

            IDictionary<string, string> cancelBookingQueryParams = input as IDictionary<string, string>;
            if (cancelBookingQueryParams == null)
            {
                return null;
            }

            AsyncClient Client = GetServiceClient(Constant.URL_CancelByBooking);
            return Client.ConsumeGetServicePrioHub(GetHttpRequestHeaders(isNonThailandProduct), cancelBookingQueryParams);


        }

        protected override async Task<object> GetResultsAsync(object input, string authString,bool isNonThailandProduct)
        {
            if (input != null)
            {
                var client = new AsyncClient
                {
                    ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GlobalTixBaseUrl)}{Constant.URL_CancelByBooking}"
                };
                return await client.PostJsonAsync<CancelReleaseRequest>(authString, (CancelReleaseRequest)input);
            }
            return null;
        }

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }
    }
}
