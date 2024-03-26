using ServiceAdapters.HotelBeds.HotelBeds.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.HotelBeds.HotelBeds.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        object Execute(InputContext inputContext, string token, out string request, out string response);

        object Execute(InputContext inputContext, string token);

        Task<object> ExecuteAsync(InputContext inputContext, string token);

        object ExecuteWithoutResponse(InputContext inputContext, string token);

        Task<object> ExecuteWithoutResponseAsync(InputContext inputContext, string token);
    }
}