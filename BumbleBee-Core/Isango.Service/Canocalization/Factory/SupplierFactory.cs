using Autofac;
using Isango.Entities.Enums;
using Isango.Service.Contract;
using Isango.Service.SupplierServices;

namespace Isango.Service.Factory
{
    public class SupplierFactory
    {
        private readonly ILifetimeScope _lifetimeScope;

        public SupplierFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public SupplierFactory()
        {
        }

        public ISupplierService GetSupplierService(APIType apiType)
        {
            if (_lifetimeScope.IsRegisteredWithKey<ISupplierService>(apiType))
                return _lifetimeScope.ResolveKeyed<ISupplierService>(apiType);
            return null;
        }
    }
}