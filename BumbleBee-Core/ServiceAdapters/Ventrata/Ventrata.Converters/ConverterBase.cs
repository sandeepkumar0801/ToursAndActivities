using Isango.Entities;
using Logger.Contract;
using ServiceAdapters.Ventrata.Ventrata.Entities;

namespace ServiceAdapters.Ventrata.Ventrata.Converters
{
    public abstract class ConverterBase
    {
        public ILogger _logger;

        public ConverterBase(ILogger logger)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _logger = logger;
        }

        public MethodType Converter { get; set; }

        public abstract object Convert(object objectResponse, object criteria);
        public abstract object Convert(object objectResponse);

        protected PricingUnit GetPricingUnitInstance(Isango.Entities.Enums.PassengerType psgrType)
        {
            switch (psgrType)
            {
                case Isango.Entities.Enums.PassengerType.Adult:
                    return new AdultPricingUnit();
                case Isango.Entities.Enums.PassengerType.Child:
                    return new ChildPricingUnit();
                case Isango.Entities.Enums.PassengerType.Infant:
                    return new InfantPricingUnit();
                case Isango.Entities.Enums.PassengerType.Senior:
                    return new SeniorPricingUnit();
                case Isango.Entities.Enums.PassengerType.Youth:
                    return new YouthPricingUnit();
                    //TODO-ToDicsuss Confusion between family and family2
                case Isango.Entities.Enums.PassengerType.Family:
                    return new FamilyPricingUnit();
                case Isango.Entities.Enums.PassengerType.Military:
                    return new MilitaryPricingUnit();
                case Isango.Entities.Enums.PassengerType.Student:
                    return new StudentPricingUnit();
                default:
                    return null;
            }
        }
    }
}
