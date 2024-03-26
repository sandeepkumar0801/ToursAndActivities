using ServiceAdapters.MoulinRouge.MoulinRouge.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Commands.Contracts
{
    public interface ICommandHandler
    {
        Task<object> ExecuteAsync<T>(T inputContext, MethodType methodType, string token);

        object Execute<T>(T inputContext, MethodType methodType, string token);

        object Execute<T>(T inputContext, MethodType methodType, string token, out string request, out string response);
    }
}