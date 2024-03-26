using Logger.Contract;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceAdapters.GlobalTixV3.Constants;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands
{
    public class BookingDetailCommandHandler : CommandHandlerBase, IBookingDetailCommandHandler
    {
        #region Constructors
        public BookingDetailCommandHandler(ILogger iLog) : base(iLog)
        {
        }
        #endregion

        protected override object CreateInputRequest(InputContext inputContext)
        {

            var bookCtx = inputContext as BookInputContextV3;
            if (bookCtx == null)
            {
                return null;
            }
            var queryParams = new Dictionary<string, string>();
            queryParams.Add(Constant.QueryParam_ReferenceNumber, bookCtx.BookingReferenceNumber.ToString());
            return queryParams;
         }

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }

        protected override object GetResults(object input, string authString,bool isNonThailandProduct)
        {
            var queryParams = input as IDictionary<string, string>;
            if (queryParams == null)
            {
                return null;
            }

            AsyncClient client = GetServiceClient(Constant.URL_BookingDetail);
            return client.ConsumeGetServicePrioHub(GetHttpRequestHeaders(isNonThailandProduct), queryParams);
        }

        protected override Task<object> GetResultsAsync(object input, string authString,bool isNonThailandProduct)
        {
            if (input == null) return null;

            var client = GetServiceClient(Constant.URL_BookingDetail);
            IDictionary<string, string> queryParams = new Dictionary<string, string>();

            return client.ConsumeGetServiceAsync(GetHttpRequestHeaders(isNonThailandProduct), queryParams);
        }
    }
}
