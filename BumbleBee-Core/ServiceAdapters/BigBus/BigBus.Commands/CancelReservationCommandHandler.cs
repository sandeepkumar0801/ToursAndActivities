using Logger.Contract;
using ServiceAdapters.BigBus.BigBus.Commands.Contracts;
using ServiceAdapters.BigBus.BigBus.Entities;
using ServiceAdapters.BigBus.Constants;
using System.Text;
using Util;

namespace ServiceAdapters.BigBus.BigBus.Commands
{
    public class CancelReservationCommandHandler : CommandHandlerBase, ICancelReservationCommandHandler
    {
        public CancelReservationCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var inputData = new CancelReservationRequestObject
            {
                CancelReservationRequest = new CancelReservationRequest
                {
                    ReservationReference = inputContext.BookingReference
                }
            };

            return inputData;
        }

        protected override string BigBusApiRequest<T>(T inputContext)
        {
            var CancelReservation = SerializeDeSerializeHelper.Serialize(inputContext);
            var Content = new StringContent(CancelReservation, Encoding.UTF8, Constant.ApplicationJson);
            var Result = HttpClient.PostAsync(UriConstants.CancelReservation, Content);
            Result.Wait();
            return Result.Result.Content.ReadAsStringAsync().Result;
        }
    }
}