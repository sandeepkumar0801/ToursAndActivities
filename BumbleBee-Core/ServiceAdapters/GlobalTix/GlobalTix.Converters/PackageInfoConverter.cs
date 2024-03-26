using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Converters.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using System.Text;

namespace ServiceAdapters.GlobalTix.GlobalTix.Converters
{
    class PackageInfoConverter : ConverterBase, IPackageInfoConverter
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

        protected Activity ConvertInternal(List<PackageInfo> packageData, PackageInfoInputContext inputContext)
        {
            var pkg = new Activity();

			//------ Activity class variables assignment
			//pkg.Id = $"{Constant.Package_Prefix}{packageData[0].Id.ToString()}";
			pkg.Id = packageData[0].Id.ToString();
			//pkg.Schedule = packageData.OpHours;
			pkg.IsPaxDetailRequired = false;

            ActivityItinerary actItin = new ActivityItinerary
            {
                Description = packageData[0].Desc,
                Title = packageData[0].Name,
                Order = 0
            };
            pkg.Itineraries = new List<ActivityItinerary> { actItin };

            // TODO: Verify that ActivityCategoryType.Attractions is correct assignment here.
            // There is no Package type in ActivityCategoryType
            pkg.CategoryTypes = new List<ActivityCategoryType> { ActivityCategoryType.Attractions };

			//------ ActivityLite class variables assignment
			//pkg.Code = $"{Constant.Package_Prefix}{packageData[0].Id.ToString()}";
			pkg.Code = packageData[0].Id.ToString();
			// TODO: Set value for DurationString. Need to check what value to set here.
			pkg.FactsheetId = inputContext.FactSheetId;

            //------ Product class variables assignment
            pkg.IsPackage = true;
            pkg.ProductType = ProductType.Ticket;
            pkg.ID = packageData[0].Id;
			SetImagePaths(pkg, packageData[0].ImagePath);

            pkg.ProductOptions = new List<ProductOption>();

            ActivityOption actOpt = new ActivityOption();
            actOpt.Id = packageData[0].Id;
            actOpt.ServiceOptionId = inputContext.ServiceOptionID ?? 0;
			//actOpt.Code = $"{Constant.Package_Prefix}{packageData[0].Id.ToString()}";
			actOpt.Code = packageData[0].linkId.ToString();
			//Assumption is that multiple packages for the same linkId will have same name
			actOpt.Name = packageData[0].Name;
            actOpt.SupplierName = Constant.Const_GTSupplierName;
            actOpt.Description = string.Empty;
            actOpt.TravelInfo = new TravelInfo() { NoOfPassengers = new Dictionary<PassengerType, int>() };
            decimal totalAmount = 0;

            // Add questions for Package
            if (packageData[0].Questions != null && packageData[0].Questions.Count > 0)
			{
				actOpt.ContractQuestions = ConvertQuestionsToContractQuestions(packageData[0].Questions);
			}

            PriceAndAvailability dftPriceAndAvail = new DefaultPriceAndAvailability() { AvailabilityStatus = AvailabilityStatus.AVAILABLE};
            dftPriceAndAvail.PricingUnits = new List<PricingUnit>();

            StringBuilder strBldr = new StringBuilder();
            foreach (PackageInfo pkgInfo in packageData)
            {
                if (pkgInfo.Variation.Name == null || Constant.Mapper_PassengerType.TryGetValue(pkgInfo.Variation.Name, out PassengerType psgrType) == false)
                {
                    continue;
                }

                PricingUnit prcUnit = GetPricingUnitInstance(psgrType);
                prcUnit.Price = pkgInfo.PayableAmount ?? 0;

                dftPriceAndAvail.PricingUnits.Add(prcUnit);
                if (prcUnit is PerPersonPricingUnit)
                {
                    PerPersonPricingUnit perPaxPrcUnit = prcUnit as PerPersonPricingUnit;
                    actOpt.TravelInfo.NoOfPassengers.Add(perPaxPrcUnit.PassengerType, 1);
					strBldr.Append($"{(int)perPaxPrcUnit.PassengerType}:{pkgInfo.Id}:");
                    if (actOpt.TravelInfo.NoOfPassengers.ContainsKey(perPaxPrcUnit.PassengerType))
                    {
                        totalAmount += actOpt.TravelInfo.NoOfPassengers[perPaxPrcUnit.PassengerType] * prcUnit.Price;
                    }
                }
            }

            Price basePrice = new Price();
            // TODO: Assign proper values here
            basePrice.Amount = totalAmount;
			// Assumption is that multiple packages for the same linkId will have same currency
			basePrice.Currency = new Currency { IsoCode = packageData[0].CurrencyCode };
            basePrice.DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability> { { DateTime.Now, dftPriceAndAvail } };
            for (var date = 0; date< inputContext.Days2Fetch; date++ )
            {
                basePrice.DatePriceAndAvailabilty.Add(inputContext.CheckinDate.AddDays(date), dftPriceAndAvail);
            }
            
            actOpt.BasePrice = basePrice;
            actOpt.CostPrice = basePrice.DeepCopy();

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

            if (actOpt?.BasePrice?.Amount != null && actOpt?.BasePrice?.Amount > 0)
            {
                actOpt.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
            }

            pkg.ProductOptions.Add(actOpt);
            pkg.ApiType = APIType.GlobalTix;

            return pkg as Activity;
        }

    }
}
