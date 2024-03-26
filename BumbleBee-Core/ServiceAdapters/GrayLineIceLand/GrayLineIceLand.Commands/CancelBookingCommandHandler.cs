using Logger.Contract;
using ServiceAdapters.GrayLineIceLand.Constants;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands.Contracts;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels;
using System;
using System.Threading.Tasks;
using Util;
using EntityConstants = ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.Constants;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands
{
    public class CancelBookingCommandHandler : CommandHandlerBase, ICancelBookingCommandHandler
    {
        public CancelBookingCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var bookingIds = new int[1];
            bookingIds[0] = Convert.ToInt32(inputContext.BookingNumber);
            return bookingIds;
        }

        protected override object GetResults(object input, string authString)
        {
            if (input == null) return null;
            var bookingNumber = (int[])input;
            var client = new AsyncClient
            {
                ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineBaseUrl)}{EntityConstants.BookingURL}/{bookingNumber[0]}"
            };
            return client.DelJson(authString, new int[0]);
        }

        protected override object GetResults(object input, string authString, out string requestXml, out string responseXml)
        {
            requestXml = "";
            responseXml = "";

            if (input == null) return null;
            var bookingNumber = (int[])input;
            var client = new AsyncClient
            {
                ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineBaseUrl)}{EntityConstants.BookingURL}/{bookingNumber[0]}"
            };
            var response = client.DelJson(authString, new int[0]);
            requestXml = bookingNumber[0].ToString();
            responseXml = SerializeDeSerializeHelper.Serialize(response);
            return response;
        }

        protected override async Task<object> GetResultsAsync(object input, string authString)
        {
            if (input == null)
                return null;
            var bookingNumber = (int[])input;
            var client = new AsyncClient
            {
                ServiceUri = new Uri($"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineBaseUrl)}{EntityConstants.BookingURL}/{bookingNumber[0]}")
            };
            return await client.DelJsonAsync(authString, new int[0]);
        }

        protected override async Task<string> GetStringResultsAsync(object input)
        {
            return null;
        }
    }
}