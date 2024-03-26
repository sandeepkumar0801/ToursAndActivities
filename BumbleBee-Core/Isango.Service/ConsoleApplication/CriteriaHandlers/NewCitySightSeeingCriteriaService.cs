using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.Enums;
using Isango.Entities.NewCitySightSeeing;
using Isango.Entities.Rezdy;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.NewCitySightSeeing;
using ServiceAdapters.Rezdy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class NewCitySightSeeingCriteriaService : INewCitySightSeeingCriteriaService
    {
        private readonly INewCitySightSeeingAdapter _newCitySightSeeingAdapter;
        private readonly ILogger _log;

        #region Class Constructors
        public NewCitySightSeeingCriteriaService(INewCitySightSeeingAdapter newCitySightSeeingAdapter, ILogger logger)
        {
            _newCitySightSeeingAdapter = newCitySightSeeingAdapter;
            _log = logger;
        }
        #endregion

        public List<Activity> GetAvailability(Entities.ConsoleApplication.ServiceAvailability.Criteria criteria)
        {
            List<Activity> activitiesList = new List<Activity>();
            var mappedProductsGroupBy = criteria.MappedProducts.GroupBy(x => x.HotelBedsActivityCode).ToList();
            try
            {
                foreach (var mappedProductData in mappedProductsGroupBy)
                {
                    try
                    {
                        foreach (var mappedProduct in mappedProductData)
                        {
                            var activity = new Activity
                            {
                                Code = "",
                                ID = mappedProduct.IsangoHotelBedsActivityId,
                                Id = mappedProduct.IsangoHotelBedsActivityId.ToString(),
                                FactsheetId = 0,
                                CurrencyIsoCode = mappedProduct.CurrencyISOCode,
                                ApiType = mappedProduct.ApiType,
                                Margin = new Margin
                                {
                                    CurrencyCode = mappedProduct.CurrencyISOCode,
                                    IsPercentage = mappedProduct.IsMarginPercent,
                                    Value = mappedProduct.MarginAmount
                                },

                                ShortName = mappedProduct.HotelBedsActivityCode,
                                CategoryIDs = new List<int> { 1 },
                            };

                            var newCitySightSeeingCriteria = new NewCitySightSeeingCriteria()
                            {
                                ActivityId = mappedProduct.HotelBedsActivityCode,
                                ServiceOptionID = 0,
                                FactSheetId = mappedProduct.FactSheetId,
                                Days2Fetch = (criteria.Days2Fetch * criteria.Months2Fetch),
                                CheckinDate = DateTime.Now,
                                ModalityCode = mappedProduct.PrefixServiceCode,
                                IsangoActivityId = Convert.ToString(mappedProduct.IsangoHotelBedsActivityId),
                                ProductMapping = new List<IsangoHBProductMapping> { mappedProduct },
                                SupplierOptionNewCitySeeing = mappedProduct.HotelBedsActivityCode
                            };

                            newCitySightSeeingCriteria.NoOfPassengers = new Dictionary<Entities.Enums.PassengerType, int>();
                            newCitySightSeeingCriteria.NoOfPassengers.Add(Entities.Enums.PassengerType.Adult, mappedProduct.MinAdultCount);
                            string request = "";
                            string response = "";
                            var getAPIProductOptions = _newCitySightSeeingAdapter.
                               GetActivityInformation
                               (newCitySightSeeingCriteria, criteria.Token,
                               out request, out response);
                            activity.ProductOptions = new List<ProductOption>();
                            activity.ProductOptions = getAPIProductOptions;

                            //match with isango data
                            foreach (IsangoHBProductMapping mappedProductItem in mappedProductData)
                            {
                                var databasePrefixData = mappedProductItem.PrefixServiceCode;
                                var apiData = activity.ProductOptions?.Where(x => x.PrefixServiceCode == databasePrefixData)?.FirstOrDefault();

                                if (apiData != null)
                                {
                                    apiData.ServiceOptionId = mappedProductItem.ServiceOptionInServiceid;
                                }
                            }
                            var finalFilterOptions = activity.ProductOptions?.Where(x => x.ServiceOptionId != 0)?.ToList();
                            activity.ProductOptions = finalFilterOptions;
                            if (activity != null)
                            {
                                activitiesList.Add(activity);
                            }
                        }
                        //var mappedProduct = mappedProductData?.FirstOrDefault();


                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return activitiesList;
        }

        public List<TempHBServiceDetail> GetServiceDetails(List<Activity> activities,
            List<IsangoHBProductMapping> mappedProducts)
        {
            var mappedProductsPass = new IsangoHBProductMapping();
            var tempServiceDetails = new List<TempHBServiceDetail>();
            var svcMapper = new ServiceMapper();

            foreach (Activity activity in activities)
            {
                mappedProductsPass = mappedProducts.FirstOrDefault(mp => mp.IsangoHotelBedsActivityId.ToString() == activity.Id);
                //if (mappedProductsPass == null)
                //{
                    //mappedProductsPass = mappedProducts.FirstOrDefault(mp => mp.HotelBedsActivityCode.ToString() == activity.Code);
                    //if (mappedProductsPass == null)
                    //{
                        //mappedProductsPass = mappedProducts?.FirstOrDefault(mp => mp.ServiceOptionInServiceid == activity.FactsheetId);
                    //}
               //}
                if (mappedProductsPass != null)
                {
                    List<TempHBServiceDetail> svcDetailsForActivity = svcMapper.ProcessServiceDetailsWithBaseGateBaseAndCostPrices(activity, mappedProductsPass);
                    if (svcDetailsForActivity != null)
                    {
                        tempServiceDetails.AddRange(svcDetailsForActivity);
                    }
                }
            }

            return tempServiceDetails;
        }
    }
}