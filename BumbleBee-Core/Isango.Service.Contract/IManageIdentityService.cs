using Isango.Entities;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface IManageIdentityService
    {
        Task<string> SubscribeNewsLetterAsync(NewsLetterData criteria);
    }
}