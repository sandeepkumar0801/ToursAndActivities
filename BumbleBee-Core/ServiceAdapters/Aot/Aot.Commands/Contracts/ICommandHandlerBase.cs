using ServiceAdapters.Aot.Aot.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.Aot.Aot.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        string AgentId { get; set; }
        string Password { get; set; }

        object Execute<T>(T inputContext, MethodType methodType, string token);

        object Execute<T>(T inputContext, MethodType methodType, string token, out string request, out string response);

        object Execute<T>(T inputContext, MethodType methodType, string token, string referenceNumber, out string request, out string response);

        Task<object> ExecuteAsync<T>(T inputContext, MethodType methodType, string token);

        string SerializeXml<T>(T requestObject);
    }
}