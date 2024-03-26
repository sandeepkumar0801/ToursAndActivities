using Logger.Contract;
using ServiceAdapters.Aot.Aot.Commands.Contracts;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using System.Text;
using Constant = ServiceAdapters.Aot.Constants.Constant;

namespace ServiceAdapters.Aot.Aot.Commands
{
    public class CancelEntireBookingCommandHandler : CommandHandlerBase, ICancelEntireBookingCommandHandler
    {
        public CancelEntireBookingCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object AotApiRequest<T>(T inputContext)
        {
            var cancelServices = inputContext as CancelServicesRequest;
            if (cancelServices == null) return null;

            cancelServices.AgentId = AgentId;
            cancelServices.Password = Password;

            var content = new StringContent(SerializeXml(cancelServices), Encoding.UTF8, Constant.ApplicationMediaType);
            var result = HttpClient.PostAsync(string.Empty, content);
            result.Wait();
            return result.Result.Content.ReadAsStringAsync().Result;
        }

        protected override async Task<object> AotApiRequestAsync<T>(T inputContext)
        {
            var cancelServices = inputContext as CancelServicesRequest;
            if (cancelServices == null) return null;

            cancelServices.AgentId = AgentId;
            cancelServices.Password = Password;

            var content = new StringContent(SerializeXml(cancelServices), Encoding.UTF8, Constant.ApplicationMediaType);
            var result = await HttpClient.PostAsync(string.Empty, content);
            return result.Content.ReadAsStringAsync().Result;
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            var request = new CancelServicesRequest
            {
                AgentId = AgentId,
                Password = Password,
                Ref = inputContext.ToString()
            };
            return request;
        }
    }
}