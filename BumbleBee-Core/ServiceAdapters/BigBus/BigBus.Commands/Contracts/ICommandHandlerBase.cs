using ServiceAdapters.BigBus.BigBus.Entities;

namespace ServiceAdapters.BigBus.BigBus.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        string Execute(InputContext inputContext, MethodType methodType, string token, out string request, out string response);

        //Task<object> ExecuteAsync(InputContext inputContext, MethodType methodType, string token);
    }
}