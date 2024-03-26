using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.PrioTicket.PrioTicket.Commands.Contracts
{
    public interface ICommandHandler
    {
        object Execute(InputContext inputContext, string token, string apiLoggingToken);

        object Execute(InputContext inputContext, string token, string apiLoggingToken, out string requestXml,
            out string responseXml);

        Task<object> ExecuteAsync(InputContext inputContext, string token, string apiLoggingToken);
    }
}