using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels;
using System.Threading.Tasks;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands.Contracts
{
    public interface ICommandHandler
    {
        Task<object> ExecuteAsync(InputContext inputContext, string token);

        Task<object> ExecuteCancelAsync(InputContext inputContext, string token);

        object ExecuteCancel(InputContext inputContext, string token, out string requestXml, out string responseXml);

        object Execute(InputContext inputContext, string token, out string request, out string response);

        object Execute(InputContext inputContext, string token);

        Task<string> ExecuteStringAsync(InputContext inputContext, string token);
    }
}