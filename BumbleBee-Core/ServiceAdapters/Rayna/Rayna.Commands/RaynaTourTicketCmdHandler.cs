using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Entities;
using Util;

namespace ServiceAdapters.Rayna.Rayna.Commands
{
    public class RaynaTourTicketCmdHandler : CommandHandlerBase, IRaynaTourTicketCmdHandler
    {
        public RaynaTourTicketCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T bookingContext)
        {
            
            var inputContext = bookingContext as ServiceAdapters.Rayna.Rayna.Entities.InputContext;
            var returnData = new TourTicketREQ
            {
               BookedOption=new List<Bookedoption>(),
               ReferenceNo= inputContext?.ReferenceNo,
               UniqNO= inputContext?.UniqueNo
            };
            var BookedoptionGet = new Bookedoption
            {
                BookingId = inputContext.BookingId,
                ServiceUniqueId = inputContext?.ServiceUniqueId
            };
            if (BookedoptionGet != null)
            {
                returnData.BookedOption.Add(BookedoptionGet);
            }
            return returnData;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(TourTicketRES);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<TourTicketRES>(responseText);
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
            var data = input as TourTicketREQ;
            var url = string.Format($"{_raynaURL}{Constants.Constants.GetBookedTickets}");
            var response = GetResponseFromAPIEndPoint(data, url);
            return response;
        }
    }
}