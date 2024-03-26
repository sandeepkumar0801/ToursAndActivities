using Isango.Entities;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.GlobalTixV3.Constants;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands
{
    public class BookingCommandHandler : CommandHandlerBase, IBookingCommandHandler
    {
        public BookingCommandHandler(ILogger iLog) : base(iLog)
        {
           
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var bookCtx = inputContext as BookInputContextV3;
            if (bookCtx == null)
            {
                return null;
            }

            var bookRQ = new BookingRQ() { ReferenceNumber = bookCtx.BookingReferenceNumber };
            return bookRQ;
        }

        protected override object GetResults(object input, string authString, bool isNonThailandProduct)
        {
            var bookRQ = input as BookingRQ;
            if (bookRQ == null)
            {
                return null;
            }

            AsyncClient client = GetServiceClient(Constant.URL_FinalBooking);
            return client.ConsumePostServiceHttpResponse(GetHttpRequestHeaders(isNonThailandProduct), Constant.HttpHeader_ContentType_JSON, SerializeDeSerializeHelper.Serialize(bookRQ), Encoding.UTF8);
        }

        protected override async Task<object> GetResultsAsync(object input, string authString, bool isNonThailandProduct)
        {
            if (input != null)
            {
                var client = new AsyncClient
                {
                    ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GlobalTixBaseUrl)}{Constant.URL_FinalBooking}"
                };
                return await client.PostJsonAsync<ReservationRQ>(authString, (ReservationRQ)input);
            }
            return null;
        }

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }
    }
}
