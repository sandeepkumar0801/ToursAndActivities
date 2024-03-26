using ServiceAdapters.PrioHub.PrioHub.Entities;

namespace ServiceAdapters.PrioHub.PrioHub.Commands.Contract
{
    public interface ICommandHandlerBase
    {
         object Execute<T>(T inputContext, string token, MethodType methodType, out string request, out string response);
    }
}