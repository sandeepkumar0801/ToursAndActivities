using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands.Contract
{
    public interface ICommandHandlerBase
    {
        Task<object> ExecuteAsync<T>(T inputContext, string token, MethodType methodType);

        object Execute<T>(T inputContext, string token, MethodType methodType, out string request, out string response);
    }
}