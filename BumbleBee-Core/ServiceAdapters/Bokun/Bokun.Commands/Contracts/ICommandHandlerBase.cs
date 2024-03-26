using ServiceAdapters.Bokun.Bokun.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.Bokun.Bokun.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        object Execute<T>(T inputContext, MethodType methodType, string token);

        object Execute<T>(T inputContext, MethodType methodType, string token, out string apiRequest, out string apiResponse);

        Task<object> ExecuteAsync<T>(T inputContext, MethodType methodType, string token);
    }
}