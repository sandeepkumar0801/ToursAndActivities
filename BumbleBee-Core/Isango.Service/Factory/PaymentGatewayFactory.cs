using Autofac;
using Isango.Entities.Enums;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Isango.Service.PaymentServices;
using Logger.Contract;
using ServiceAdapters.Adyen;
using ServiceAdapters.Apexx;
using ServiceAdapters.WirecardPayment;

namespace Isango.Service.Factory
{
    public class PaymentGatewayFactory
    {
        private ILifetimeScope _lifetimeScope;
        private IWirecardPaymentAdapter _wirecardPaymentAdapter;
        private IApexxAdapter _apexxPaymentAdapter;
        private IAdyenAdapter _adyenPaymentAdapter;
        private ILogger _log;
        private IAdyenPersistence _adyenPersistence;

        public PaymentGatewayFactory(ILifetimeScope lifetimeScope,
            IWirecardPaymentAdapter wirecardPaymentAdapter,
            IApexxAdapter apexxPaymentAdapter,
            ILogger log, IAdyenAdapter adyenPaymentAdapter,
            IAdyenPersistence adyenPersistence
        )
        {
            _lifetimeScope = lifetimeScope;
            _wirecardPaymentAdapter = wirecardPaymentAdapter;
            _apexxPaymentAdapter = apexxPaymentAdapter;
            _log = log;
            _adyenPaymentAdapter = adyenPaymentAdapter;
            _adyenPersistence = adyenPersistence;
        }

        public IPaymentService GetPaymentGatewayService(PaymentGatewayType paymentGateway)
        {
            try
            {
                if (_lifetimeScope.IsRegisteredWithKey<IPaymentService>(paymentGateway))
                    return _lifetimeScope.ResolveKeyed<IPaymentService>(paymentGateway);
            }
            catch (System.Exception ex)
            {
                switch (paymentGateway)
                {
                    case PaymentGatewayType.WireCard:
                        {
                            return new WirecardPaymentService(_wirecardPaymentAdapter, _log);
                        }
                    case PaymentGatewayType.Apexx:
                        {
                            return new ApexxPaymentService(_apexxPaymentAdapter, _log);
                        }
                    case PaymentGatewayType.Adyen:
                        {
                            return new AdyenPaymentService(_adyenPaymentAdapter, _log, _adyenPersistence);
                        }
                }
            }
            return null;
        }
    }
}