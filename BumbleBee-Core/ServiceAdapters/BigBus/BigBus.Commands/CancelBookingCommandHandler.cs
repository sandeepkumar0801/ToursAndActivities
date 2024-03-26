using Logger.Contract;
using ServiceAdapters.BigBus.BigBus.Commands.Contracts;
using ServiceAdapters.BigBus.BigBus.Entities;
using ServiceAdapters.BigBus.Constants;
using System.Text;
using Util;

namespace ServiceAdapters.BigBus.BigBus.Commands
{
    public class CancelBookingCommandHandler : CommandHandlerBase, ICancelBookingCommandHandler
    {
        public CancelBookingCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var inputData = new CancelBookingRequest
            {
                CancelBookingReq = new CancelBooking
                {
                    BookingReference = inputContext.BookingReference
                }
            };

            return inputData;
        }

        protected override string BigBusApiRequest<T>(T inputContext)
        {
            var CancelBooking = SerializeDeSerializeHelper.Serialize(inputContext);
            var Content = new StringContent(CancelBooking, Encoding.UTF8, Constant.ApplicationJson);
            var Result = HttpClient.PostAsync(UriConstants.CancelBooking, Content);
            Result.Wait();
            return Result.Result.Content.ReadAsStringAsync().Result;
        }
    }
}