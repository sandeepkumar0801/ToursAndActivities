using ServiceAdapters.GoCity.GoCity.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.GoCity.GoCity.Commands.Contract
{
    public interface ICommandHandlerBase
    {
        Task<object> ExecuteAsync<T>(T inputContext, string token, MethodType methodType);

        object Execute<T>(T inputContext, string token, MethodType methodType, out string request, out string response);
    }
}