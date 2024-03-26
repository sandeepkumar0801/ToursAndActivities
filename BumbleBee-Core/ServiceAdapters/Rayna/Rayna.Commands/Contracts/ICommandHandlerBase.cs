using ServiceAdapters.Rayna.Rayna.Entities;

namespace ServiceAdapters.Rayna.Rayna.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        object Execute<T>(T inputContext, string token, MethodType methodType, out string request, out string response);

        object Execute<T>(T inputContext, string token, MethodType methodType);
    }
}