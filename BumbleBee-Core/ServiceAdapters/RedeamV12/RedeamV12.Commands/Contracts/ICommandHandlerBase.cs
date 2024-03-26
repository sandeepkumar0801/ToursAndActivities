using ServiceAdapters.RedeamV12.RedeamV12.Entities;
using System.Net;
using System.Threading.Tasks;

namespace ServiceAdapters.RedeamV12.RedeamV12.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        object Execute<T>(T inputContext, MethodType methodType, string token);

        object Execute<T>(T inputContext, MethodType methodType, string token, out string request, out string response, out HttpStatusCode httpStatusCode);

        Task<object> ExecuteAsync<T>(T inputContext, MethodType methodType, string token);
    }
}