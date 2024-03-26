using ServiceAdapters.Tiqets.Tiqets.Entities;
using System.Net;

namespace ServiceAdapters.Tiqets.Tiqets.Commands.Contracts
{
    public interface ICommandHandler
    {
        object Execute<T>(T inputContext, MethodType methodType, string token);

        object Execute<T>(T inputContext, string token, MethodType methodType, out string apiRequest, out string apiResponse, out HttpStatusCode httpStatusCode);
    }
}