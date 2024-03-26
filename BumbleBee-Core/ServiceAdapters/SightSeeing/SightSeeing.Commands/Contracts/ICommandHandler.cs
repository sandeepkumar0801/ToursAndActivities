using ServiceAdapters.SightSeeing.SightSeeing.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.SightSeeing.SightSeeing.Commands.Contracts
{
    public interface ICommandHandler
    {
        string Execute(InputContext inputContext, MethodType methodType, string token, out string requestXml, out string responseXml);

        Task<string> ExecuteAsync(InputContext inputContext, MethodType methodType, string token);

        string Execute(InputContext inputContext, MethodType methodType, string token);
    }
}