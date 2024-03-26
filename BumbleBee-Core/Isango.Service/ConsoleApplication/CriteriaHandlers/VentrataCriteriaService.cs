using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.Enums;
using Isango.Entities.Ventrata;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Ventrata;
using System;
using System.Collections.Generic;
using System.Linq;

using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class VentrataCriteriaService : IVentrataCriteriaService
    {
        private readonly IVentrataAdapter _ventrataAdapter;
        private readonly ILogger _log;
        private readonly IMasterCacheManager _masterCacheManager;
        private readonly IMasterPersistence _masterPersistence;
        private string _className;

        #region Class Constructors

        public VentrataCriteriaService(IVentrataAdapter ventrataAdapter, ILogger logger, IMasterCacheManager masterCacheManager, IMasterPersistence masterPersistence)
        {
            _ventrataAdapter = ventrataAdapter;
            _log = logger;
            _masterCacheManager = masterCacheManager;
            _masterPersistence = masterPersistence;
            _className = nameof(VentrataCriteriaService);
        }

        #endregion Class Constructors

        public List<Activity> GetAvailability(ServiceAvailability.Criteria criteria, List<Entities.Ventrata.SupplierDetails> supplierDetails)
        {
            List<Activity> activitiesList = new List<Activity>();

            var allPaxMappings = _masterCacheManager.GetVentrataPaxMappings();
            if (allPaxMappings == null)
            {
                allPaxMappings = _masterPersistence.GetVentrataPaxMappings()?.Where(x => x.APIType == APIType.Ventrata).ToList();
            }

            foreach (IsangoHBProductMapping mappedProduct in criteria.MappedProducts)
            {
                var ventrataOptionCodeAndProductId = mappedProduct.ServiceOptionInServiceid + "*" + mappedProduct.HotelBedsActivityCode + "*" + mappedProduct.PrefixServiceCode;
                VentrataAvailabilityCriteria ventrataAvailCriteria = new VentrataAvailabilityCriteria()
                {
                    ProductId = mappedProduct.PrefixServiceCode,
                    SupplierOptionCodesAndProductIdVsApiOptionIds = new Dictionary<string, List<string>>(),
                    CheckinDate = DateTime.Now.Date,
                    Token = criteria.Token,
                    CheckoutDate = DateTime.Now.AddDays(criteria.Days2Fetch * criteria.Months2Fetch).Date,
                    SupplierBearerToken = mappedProduct.SupplierCode,
                    IsSupplementOffer = true
                };
                if (supplierDetails != null && supplierDetails.Count > 0)
                {
                    var getBaseURL = supplierDetails?.Where(x => x.SupplierBearerToken == mappedProduct.SupplierCode)?.Select(x => x.BaseURL)?.FirstOrDefault();
                    if (!String.IsNullOrEmpty(getBaseURL))
                    {
                        ventrataAvailCriteria.VentrataBaseURL = getBaseURL;
                    }
                }

                ventrataAvailCriteria.VentrataPaxMappings = allPaxMappings?.Where(x => x.ServiceOptionId == mappedProduct.ServiceOptionInServiceid).ToList();
                //var noOfPassengers = paxMappings?.Select(e => e.PassengerType).Distinct().ToDictionary(x => x, x => mappedProduct.MinAdultCount);

                ventrataAvailCriteria.NoOfPassengers = new Dictionary<Entities.Enums.PassengerType, int>();
                ventrataAvailCriteria.NoOfPassengers.Add(Entities.Enums.PassengerType.Adult, mappedProduct.MinAdultCount);
                ventrataAvailCriteria.SupplierOptionCodesAndProductIdVsApiOptionIds.Add(ventrataOptionCodeAndProductId, new List<string>());

                List<Activity> activities = _ventrataAdapter.GetOptionsForVentrataActivity(ventrataAvailCriteria, ventrataAvailCriteria.Token);
                if (activities != null && activities.Count > 0)
                {
                    activities[0].ID = mappedProduct.IsangoHotelBedsActivityId;
                    activitiesList.Add(activities[0]);
                }
            }

            return activitiesList;
        }

        public List<TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts)
        {
            List<TempHBServiceDetail> tempServiceDetails = new List<TempHBServiceDetail>();
            ServiceMapper svcMapper = new ServiceMapper();
            try
            {
                foreach (Activity activity in activities)
                {
                    try
                    {
                        if (activity.ProductOptions != null && activity.ProductOptions.Count > 0)
                        {
                            //var ventrataActOption = activity.ProductOptions[0] as ActivityOption;
                            IsangoHBProductMapping mappedProduct = mappedProducts.FirstOrDefault(mp => mp.IsangoHotelBedsActivityId == activity.ID);
                            if (mappedProduct == null)
                            {
                                continue;
                            }

                            List<TempHBServiceDetail> svcDetailsForActivity = svcMapper.ProcessServiceDetailsWithBaseGateBaseAndCostPrices(activity, mappedProduct);
                            if (svcDetailsForActivity != null)
                            {
                                tempServiceDetails.AddRange(svcDetailsForActivity);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(
                            new IsangoErrorEntity
                            {
                                ClassName = _className,
                                MethodName = nameof(GetServiceDetails),
                                Params = $"ActivityId {activity.Id}"
                            }, ex
                            );
                    }
                }

                return tempServiceDetails;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}