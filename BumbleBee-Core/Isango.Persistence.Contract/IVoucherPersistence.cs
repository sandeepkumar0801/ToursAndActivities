using Isango.Entities;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IVoucherPersistence
    {
        Voucher GetVoucher(string voucherCode);

        List<DiscountCategoryConfig> GetAllDiscountCategoryConfig();
    }
}