using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using ServiceAdapters.GlobalTixV3.Constants;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Converters.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using packageData = ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.PackageOptions.PackageOptionsList;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Converters
{
    class PackageOptionsConverter : ConverterBase, IPackageOptionsConverter
    {


        public override object Convert(object objectResult)
        {
            PackageInfoInputContext pkgInfoCtx = objectResult as PackageInfoInputContext;
            if (pkgInfoCtx == null)
            {
                return null;
            }

            return ConvertInternal(pkgInfoCtx.LinkedPackages, pkgInfoCtx);
        }

        public override object Convert(object objectResult, object input)
        {
            return Convert(objectResult);
        }

        protected Activity ConvertInternal(packageData.Datum packageData, PackageInfoInputContext inputContext)
        {
            var isangoAPIMapping = inputContext.GlobalTixV3Mapping;
            if (isangoAPIMapping == null || isangoAPIMapping.Count==0)
            {
                return null;
            }
            //filter mapping by option
            isangoAPIMapping = isangoAPIMapping?.Where(x => x.SupplierCode == packageData.Id.ToString())?.ToList();

            var pkg = new Activity();

            //------ Activity class variables assignment
            pkg.Id = packageData.Id.ToString();
            //pkg.Schedule = packageData.OpHours;
            pkg.IsPaxDetailRequired = false;

            ActivityItinerary actItin = new ActivityItinerary
            {
                Description = packageData.Description,
                Title = packageData.Name,
                Order = 0
            };
            pkg.Itineraries = new List<ActivityItinerary> { actItin };

            // TODO: Verify that ActivityCategoryType.Attractions is correct assignment here.
            // There is no Package type in ActivityCategoryType
            pkg.CategoryTypes = new List<ActivityCategoryType> { ActivityCategoryType.Attractions };

            //------ ActivityLite class variables assignment

            pkg.Code = packageData.Id.ToString();
            // TODO: Set value for DurationString. Need to check what value to set here.
            pkg.FactsheetId = inputContext.FactSheetId;

            //------ Product class variables assignment
            pkg.IsPackage = true;
            pkg.ProductType = ProductType.Ticket;
            pkg.ID = packageData.Id;


            pkg.ProductOptions = new List<ProductOption>();

            ActivityOption actOpt = new ActivityOption();
            actOpt.Id = packageData.Id;
            actOpt.ServiceOptionId = inputContext.ServiceOptionID ?? 0;

            actOpt.Code = packageData.Id.ToString();
            //Assumption is that multiple packages for the same linkId will have same name
            actOpt.Name = packageData.Name;
            actOpt.SupplierName = Constant.Const_GTSupplierName;
            actOpt.Description = string.Empty;
            actOpt.TravelInfo = new TravelInfo() { NoOfPassengers = new Dictionary<PassengerType, int>() };
            decimal totalAmount = 0;
            decimal totalCostAmount = 0;

            // Add questions for Package
            //         if (packageData.Questions != null && packageData.Questions.Count > 0)
            //{
            //	actOpt.ContractQuestions = ConvertQuestionsToContractQuestions(packageData[0].Questions);
            //}

            PriceAndAvailability dftPriceAndAvail = new DefaultPriceAndAvailability() { AvailabilityStatus = AvailabilityStatus.AVAILABLE };
            dftPriceAndAvail.PricingUnits = new List<PricingUnit>();

            PriceAndAvailability dftPriceAndAvailCost = new DefaultPriceAndAvailability() { AvailabilityStatus = AvailabilityStatus.AVAILABLE };
            dftPriceAndAvailCost.PricingUnits = new List<PricingUnit>();

            StringBuilder strBldr = new StringBuilder();
            foreach (var pkgInfo in packageData.PackageType)
            {
                if (pkgInfo.PackageTypeName == null)
                {
                    continue;
                }

                string psgrTypeStr = pkgInfo.PackageTypeName?.ToUpper();
                var psgrType = isangoAPIMapping.Where(x => x.AgeGroupCode.ToUpper() == psgrTypeStr).FirstOrDefault().PassengerType;


                PricingUnit prcUnit = GetPricingUnitInstance(psgrType);
                if (pkgInfo.PackageTypeOriginalPrice != 0)
                {
                    prcUnit.Price = System.Convert.ToDecimal(pkgInfo.PackageTypeOriginalPrice.ToString("#.##"));
                }
                else
                {
                    prcUnit.Price = 0;

                }
                dftPriceAndAvail.PricingUnits.Add(prcUnit);
                if (prcUnit is PerPersonPricingUnit)
                {
                    PerPersonPricingUnit perPaxPrcUnit = prcUnit as PerPersonPricingUnit;
                    actOpt.TravelInfo.NoOfPassengers.Add(perPaxPrcUnit.PassengerType, 1);
                    strBldr.Append($"{(int)perPaxPrcUnit.PassengerType}:{pkgInfo.PackageTypeId}:");
                    if (actOpt.TravelInfo.NoOfPassengers.ContainsKey(perPaxPrcUnit.PassengerType))
                    {
                        totalAmount += actOpt.TravelInfo.NoOfPassengers[perPaxPrcUnit.PassengerType] * prcUnit.Price;
                    }
                }


                PricingUnit prcUnitCost = GetPricingUnitInstance(psgrType);
                if (pkgInfo.PackageTypeNettPrice != 0)
                {
                    prcUnitCost.Price = System.Convert.ToDecimal(pkgInfo.PackageTypeNettPrice.ToString("#.##"));
                }
                else
                {
                    prcUnitCost.Price = 0;
                }
                dftPriceAndAvailCost.PricingUnits.Add(prcUnitCost);
                if (prcUnitCost is PerPersonPricingUnit)
                {
                    PerPersonPricingUnit perPaxPrcUnitCost = prcUnitCost as PerPersonPricingUnit;
                    if (!actOpt.TravelInfo.NoOfPassengers.ContainsKey(perPaxPrcUnitCost.PassengerType))
                    {
                        actOpt.TravelInfo.NoOfPassengers.Add(perPaxPrcUnitCost.PassengerType, 1);
                        strBldr.Append($"{(int)perPaxPrcUnitCost.PassengerType}:{pkgInfo.PackageTypeId}:");
                    }
                    
                    if (actOpt.TravelInfo.NoOfPassengers.ContainsKey(perPaxPrcUnitCost.PassengerType))
                    {
                        totalCostAmount += actOpt.TravelInfo.NoOfPassengers[perPaxPrcUnitCost.PassengerType] * prcUnitCost.Price;
                    }
                }
            }

            var basePrice = new Price();
            // TODO: Assign proper values here
            basePrice.Amount = totalAmount;
            // Assumption is that multiple packages for the same linkId will have same currency
            basePrice.Currency = new Currency { IsoCode = packageData.Currency };
            basePrice.DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>();


            var costPrice = new Price();
            // TODO: Assign proper values here
            costPrice.Amount = totalCostAmount;
            // Assumption is that multiple packages for the same linkId will have same currency
            costPrice.Currency = new Currency { IsoCode = packageData.Currency };
            costPrice.DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>();

           

            for (var date = 0; date < inputContext.Days2Fetch; date++)
            {
                basePrice.DatePriceAndAvailabilty.Add(inputContext.CheckinDate.AddDays(date), dftPriceAndAvail);
                costPrice.DatePriceAndAvailabilty.Add(inputContext.CheckinDate.AddDays(date), dftPriceAndAvailCost);
            }

            actOpt.BasePrice = basePrice;
            actOpt.CostPrice = costPrice;

            // When booking a ticket, GlobalTix API needs ids of packages. As of now, in PricingUnit and its derived classes, there 
            // is no provision to store this id. Therefore, storing it in ProductOption.RateKey as a workaround
            if (strBldr.Length > 0)
            {
                // Decrease length to get rid of last ':' character
                strBldr.Length--;
                actOpt.RateKey = strBldr.ToString();
            }
            //------
            actOpt.SupplierOptionCode = strBldr.ToString();

            if (actOpt?.BasePrice?.Amount != null && actOpt?.BasePrice?.Amount >= 0)
            {
                actOpt.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
            }

            pkg.ProductOptions.Add(actOpt);
            pkg.ApiType = APIType.GlobalTixV3;

            return pkg as Activity;
        }

        public override object Convert(object objectResult, object objectResultDetail, object input)
        {
            throw new NotImplementedException();
        }
    }
}
