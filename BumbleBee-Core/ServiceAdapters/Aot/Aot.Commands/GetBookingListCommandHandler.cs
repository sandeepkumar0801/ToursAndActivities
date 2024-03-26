using Logger.Contract;
using ServiceAdapters.Aot.Aot.Commands.Contracts;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using System.Text;
using Constant = ServiceAdapters.Aot.Constants.Constant;

namespace ServiceAdapters.Aot.Aot.Commands
{
    public class GetBookingListCommandHandler : CommandHandlerBase, IGetBookingListCommandHandler
    {
        public GetBookingListCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object AotApiRequest<T>(T inputContext)
        {
            var bookingList = inputContext as ListBookingsRequest;
            if (bookingList == null) return null;

            bookingList.AgentId = AgentId;
            bookingList.Password = Password;

            var content = new StringContent(SerializeXml(bookingList), Encoding.UTF8, Constant.ApplicationMediaType);
            var result = HttpClient.PostAsync(string.Empty, content);
            result.Wait();
            return result.Result.Content.ReadAsStringAsync().Result;
        }

        protected override async Task<object> AotApiRequestAsync<T>(T inputContext)
        {
            var bookingList = inputContext as ListBookingsRequest;
            if (bookingList == null) return null;

            bookingList.AgentId = AgentId;
            bookingList.Password = Password;

            var content = new StringContent(SerializeXml(bookingList), Encoding.UTF8, Constant.ApplicationMediaType);
            var result = await HttpClient.PostAsync(string.Empty, content);
            return result.Content.ReadAsStringAsync().Result;
        }
    }
}