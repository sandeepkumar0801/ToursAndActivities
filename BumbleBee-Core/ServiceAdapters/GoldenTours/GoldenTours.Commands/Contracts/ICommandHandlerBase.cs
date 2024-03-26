using ServiceAdapters.GoldenTours.GoldenTours.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.GoldenTours.GoldenTours.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        object Execute<T>(T inputContext, MethodType methodType, string token);

        object Execute<T>(T inputContext, MethodType methodType, string token, out string request, out string response);

        Task<object> ExecuteAsync<T>(T inputContext, MethodType methodType, string token);
    }
}