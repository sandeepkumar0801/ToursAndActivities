using Isango.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface IVoucherService
    {
        Task<Voucher> GetVoucherAsync(string voucherCode);

        Task<List<DiscountCategoryConfig>> GetAllDiscountCategoryConfigAsync();
    }
}