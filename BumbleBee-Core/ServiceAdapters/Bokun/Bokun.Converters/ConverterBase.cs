using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Bokun;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Entities.CheckAvailabilities;
using CONSTANT = Util.CommonUtilConstantCancellation;
using RESOURCEMANAGER = Util.CommonResourceManager;

namespace ServiceAdapters.Bokun.Bokun.Converters
{
    public abstract class ConverterBase
    {
        protected readonly List<PassengerType> validPassengerTypes = new List<PassengerType> { PassengerType.Adult, PassengerType.Child, PassengerType.Youth, PassengerType.Infant, PassengerType.Senior, PassengerType.Concession, PassengerType.Family, PassengerType.Student };
        public ILogger _logger;

        protected ConverterBase(ILogger logger)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _logger = logger;
        }

        public void UpdateCancellationPolicy(List<CancellationPrice> cancellationPrices
            , Penaltyrule penaltyRule
            , DateTime defaultDate
            , CheckAvailabilitiesRs checkAvailability
            , ActivityOption option
            , BokunCriteria criteria
            , Cancellationpolicy cancellationpolicy
            )
        {
            var cancellationText = string.Empty;
            var languageCode = criteria?.Language?.ToLower() ?? "en";
            try
            {
                if (cancellationPrices == null)
                {
                    cancellationPrices = new List<CancellationPrice>();
                }

                var penaltyRuleCutoffHours = penaltyRule?.CutoffHours ?? 0;

                var cutOffHours = penaltyRuleCutoffHours <= 24 ? 24 : penaltyRuleCutoffHours;
                cutOffHours = penaltyRuleCutoffHours >= 72 ? 72 : cutOffHours;

                var cancellationPrice = new CancellationPrice()
                {
                    CancellationDateRelatedToOpreationDate = defaultDate.AddMilliseconds(checkAvailability.Date),
                    Percentage = penaltyRule?.Percentage ?? 0,
                    CancellationFromdate = penaltyRuleCutoffHours > 0 ? (defaultDate.AddMilliseconds(checkAvailability.Date).AddHours(-(cutOffHours))) : DateTime.Now.Date,
                    CancellationToDate = defaultDate.AddMilliseconds(checkAvailability.Date)
                };
                cancellationPrices.Add(cancellationPrice);

                //Set cancellation policy on cutoff Hours basis
                if (cutOffHours > 0)
                {
                    option.Cancellable = true;
                    option.CancellationText = $"{string.Format(RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicy100ChargableBeforeNhours), cutOffHours, cutOffHours)}";
                }
                else
                {
                    option.Cancellable = false;
                    option.CancellationText = RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicyNonRefundable);
                }
                if (cancellationpolicy?.PolicyType?.ToUpper() == "NON_REFUNDABLE")
                {
                    option.Cancellable = false;
                    option.CancellationText = RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicyNonRefundable);

                }
                foreach (var cp in cancellationPrices)
                {
                    cp.CancellationDescription = option.CancellationText;

                }
                option.CancellationPrices = cancellationPrices;
                option.ApiCancellationPolicy = Util.SerializeDeSerializeHelper.Serialize(penaltyRule);
            }
            catch (System.Exception ex)
            {
                //throw;
            }
        }
    }
}