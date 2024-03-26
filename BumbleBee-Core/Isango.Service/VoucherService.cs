using Isango.Entities;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;

namespace Isango.Service
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherPersistence _voucherPersistence;
        private readonly ILogger _log;

        public VoucherService(IVoucherPersistence voucherPersistence, ILogger log)
        {
            _voucherPersistence = voucherPersistence;
            _log = log;
        }

        /// <summary>
        /// Get voucher by voucher code
        /// </summary>
        /// <param name="voucherCode"></param>
        /// <returns></returns>
        public async Task<Voucher> GetVoucherAsync(string voucherCode)
        {
            try
            {
                return await Task.FromResult(_voucherPersistence.GetVoucher(voucherCode));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "VoucherService",
                    MethodName = "GetVoucherAsync",
                    Params = $"{voucherCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get all the discount category configurations
        /// </summary>
        /// <returns></returns>
        public async Task<List<DiscountCategoryConfig>> GetAllDiscountCategoryConfigAsync()
        {
            try
            {
                return await Task.FromResult(_voucherPersistence.GetAllDiscountCategoryConfig());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "VoucherService",
                    MethodName = "GetVoucherAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}