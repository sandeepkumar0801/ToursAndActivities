using ServiceAdapters.Apexx.Apexx.Entities;

namespace ServiceAdapters.Apexx.Apexx.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        string Execute(ApexxCriteria apexxCriteria, string token);
    }
}