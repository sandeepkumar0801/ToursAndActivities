using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface ISynchronizerService
    {
        Task<bool> PollDatabaseForChangesAsync();
    }
}