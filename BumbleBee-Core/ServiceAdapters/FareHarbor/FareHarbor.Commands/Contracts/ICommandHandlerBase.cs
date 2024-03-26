using ServiceAdapters.FareHarbor.FareHarbor.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.FareHarbor.FareHarbor.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        object Execute<T>(T inputContext, MethodType methodType, string token);

        object Execute<T>(T inputContext, MethodType methodType, string token, out string request, out string response);

        Task<object> ExecuteAsync<T>(T inputContext, MethodType methodType, string token);

        string RemoverProperties(string jSon, bool isReBooking);
    }
}