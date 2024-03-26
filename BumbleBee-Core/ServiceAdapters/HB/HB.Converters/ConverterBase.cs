using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.HotelBeds;
using System;
using System.Collections.Generic;
using System.Linq;
using CONSTANT = Util.CommonUtilConstantCancellation;
using CONSTANTCOMMON = Util.CommonUtilConstant;
using RESOURCEMANAGER = Util.CommonResourceManager;

namespace ServiceAdapters.HB.HB.Converters
{
    public abstract class ConverterBase
    {
        public ConverterBase()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }

        public decimal ApplyDefaultMargin(decimal costPrice, decimal defaultMargin = 0M)
        {
            var defaultMarginFromConfig = Util.ConfigurationManagerHelper.GetValuefromAppSettings(CONSTANTCOMMON.DefaultMargin);
            if (string.IsNullOrWhiteSpace(defaultMarginFromConfig))
            {
                defaultMarginFromConfig = "20";
            }
            if(defaultMargin <= 0)
            {
                Decimal.TryParse(defaultMarginFromConfig, out defaultMargin);
                //defaultMargin = Decimal.TryParse(defaultMarginFromConfig);
            }
            var divisor = 100 - defaultMargin;
            var divisor1 = 100/divisor;
            decimal result = costPrice * divisor1;
            return Math.Round(result, 2);
        }

        public void UpdateCancellationPolicyText(List<CancellationPrice> cancellationCosts, HotelbedCriteriaApitude criteria, ActivityOption activityOption)
        {
            var languageCode = criteria.Language.ToLower();
            try
            {
                if (cancellationCosts?.Count == 0)
                {
                    //Last minute cancellation allowed but keeping 24 hour
                    activityOption.CancellationText = $"{RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicyFreeBeforeTravelDate)}";
                    activityOption.Cancellable = true;
                    return;
                }
                cancellationCosts = cancellationCosts?.OrderBy(y => y.CancellationDateRelatedToOpreationDate).ThenBy(y => y.CancellationFromdate)?.ToList();
                var policyPercenatges = cancellationCosts?.Where(x =>
                criteria.CheckinDate.Date >= x.CancellationDateRelatedToOpreationDate.Date ||
                criteria.CheckinDate.Date <= x.CancellationToDate.Date

                )?.Select(x => x.Percentage)?.Distinct()?.OrderByDescending(y => y).ToList();

                var cancellationApplicable = cancellationCosts?.Where(x =>
                     (x.CancellationAmount > 0 || x.Percentage > 0)
                     &&
                     (
                        criteria.CheckinDate.Date >= x.CancellationDateRelatedToOpreationDate.Date ||
                        criteria.CheckinDate.Date <= x.CancellationToDate.Date
                     )
                ).ToList();

                var cancellationText = string.Empty;
                activityOption.CancellationText = string.Empty;
                activityOption.Cancellable = false;
                //if ((policyPercenatges?.Any(x => x == 100) == true) && policyPercenatges?.Count == 1)
                //{
                //    activityOption.CancellationText = Constant.CancellationPolicyNonRefundable;
                //}
                //else
                if (cancellationApplicable?.Any() == true)
                {
                    var hours = 0;
                    foreach (var pp in policyPercenatges)
                    {
                        try
                        {
                            var cc = cancellationApplicable?.OrderBy(y => y.CancellationDateRelatedToOpreationDate)?.FirstOrDefault(x => x.Percentage == pp);
                            var days = (cc.CancellationDateRelatedToOpreationDate - cc.CancellationFromdate).Days;
                            hours = days * 24;
                            if (hours > 72)
                            {
                                hours = 72;
                            }

                            if (cc.Percentage == 100 && policyPercenatges?.Count == 1)
                            {
                                if (hours > 0)
                                {
                                    if (cc.CancellationFromdate <= DateTime.Now.Date && DateTime.Now.Date <= cc.CancellationDateRelatedToOpreationDate)
                                    {
                                        cancellationText = RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicyNonRefundable);
                                    }
                                    else
                                    {
                                        cancellationText = $"{string.Format(RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicy100ChargableBeforeNhours), hours, hours)}";
                                    }
                                }
                                else
                                {
                                    cancellationText = RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicyNonRefundable);
                                }
                            }
                            else if (cc.Percentage > 0 && cc.Percentage <= 100)
                            {
                                if (hours == 0 && cc.Percentage == 100)
                                {
                                    hours = 24;

                                    activityOption.Cancellable = false;
                                }
                                else
                                {
                                    activityOption.Cancellable = true;
                                }
                                cancellationText = string.Format(RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicyChargableWithoutPercentage), hours);
                                if (activityOption?.CancellationText?.Contains(cancellationText) != true)
                                {
                                    cancellationText = string.Format(RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicyChargable), cc.Percentage, hours); ;
                                }
                            }

                            if (activityOption?.CancellationText?.Contains(cancellationText) != true)
                            {
                                activityOption.CancellationText += $"{cancellationText}";
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (policyPercenatges?.Count > 1)
                    {
                        activityOption.CancellationText += string.Format(RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicyLast), hours + 1);
                    }
                    else if (policyPercenatges?.Count == 1
                        && !activityOption.CancellationText.ToLower().Contains(RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicyNonRefundable).ToLower())
                        && !
                            (
                            activityOption.CancellationText.Contains("100%")
                            || activityOption.CancellationText.ToLower().Contains("full charges")
                            || activityOption.CancellationText.ToLower().Contains("los gastos son totales")
                            )
                        && hours > 1
                        )
                    {
                        activityOption.CancellationText += $"{RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicyFreeBeforeTravelDate)}";
                    }
                }
                else if (cancellationCosts?.FirstOrDefault(x => x.CancellationAmount == 0) != null)
                {
                    activityOption.CancellationText = RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicyDefaultFree24Hours);
                    activityOption.Cancellable = true;
                }

                foreach (var cc in cancellationCosts)
                {
                    cc.CancellationDescription = activityOption.CancellationText;
                }
            }
            catch (System.Exception ex)
            {
                //throw;
            }
        }
    }
}