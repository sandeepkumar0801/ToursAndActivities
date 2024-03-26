using Isango.Service.Contract;
using ServiceAdapters.NeverBounce;

namespace Isango.Service
{
    public class NeverBounceService : INeverBounceService
    {
        private INeverBounceAdapter _neverBounceAdapter;

        public NeverBounceService(INeverBounceAdapter neverBounceAdapter)
        {
            _neverBounceAdapter = neverBounceAdapter;
        }

        public bool IsEmailVerified(string emailId, string token)
        {
            return _neverBounceAdapter.IsEmailNbVerified(emailId, token);
        }
    }
}