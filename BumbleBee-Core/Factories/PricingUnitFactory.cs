using Isango.Entities;
using Isango.Entities.Enums;
using System;
using System.Reflection;

namespace Factories
{
    public static class PricingUnitFactory
    {
        public static PricingUnit GetPricingUnit(PassengerType passengerType)
        {
            if (passengerType == PassengerType.Undefined)
                return new PricingUnit();

            var className = $"Isango.Entities.{passengerType}PricingUnit";
            var assembly = Assembly.GetAssembly(typeof(PricingUnit));
            var type = assembly.GetType(className)?.FullName;
            return (PricingUnit)Activator.CreateInstanceFrom(assembly.Location, type).Unwrap();
        }

        public static T GetPricingUnit<T>(string className = "Isango.Entities.PerUnitPricingUnit")
        {
            var result = default(T);
            var assembly = Assembly.GetAssembly(typeof(T));
            var type = assembly.GetType(className)?.FullName;
            result = (T)Activator.CreateInstanceFrom(assembly.Location, type).Unwrap();
            return result;
        }
    }
}