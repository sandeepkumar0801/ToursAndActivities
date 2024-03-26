using CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace CacheLoader.Webjob.CustomTimerClasses
{
    public sealed class CurrencyExchangeRatesTime : CustomDailyTimerTriggerBase
    {
        public CurrencyExchangeRatesTime() : base("CurrencyExchangeRatesTime")
        {
        }
    }
}