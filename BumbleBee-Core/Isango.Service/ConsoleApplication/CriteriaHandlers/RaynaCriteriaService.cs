using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.Enums;
using Isango.Entities.Rayna;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Rayna;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class RaynaCriteriaService : IRaynaCriteriaService
    {
        private readonly IRaynaAdapter _raynaAdapter;
        private readonly ILogger _log;

        #region Class Constructors
        public RaynaCriteriaService(IRaynaAdapter raynaAdapter, ILogger logger)
        {
            _raynaAdapter = raynaAdapter;
            _log = logger;
        }
        #endregion

        public List<Activity> GetAvailability(Entities.ConsoleApplication.ServiceAvailability.Criteria criteria)
        {
            List<Activity> activitiesList = new List<Activity>();

            try
            {

                foreach (IsangoHBProductMapping mappedProduct in criteria.MappedProducts)
                {
                    try
                    {
                        var raynaCriteria = new RaynaCriteria()
                        {
                            ActivityId = mappedProduct.HotelBedsActivityCode,//API productId
                            ServiceOptionID = mappedProduct?.ServiceOptionInServiceid ?? 0,////isango option ID
                            FactSheetId = mappedProduct.FactSheetId,
                            Days2Fetch = (criteria.Days2Fetch * criteria.Months2Fetch),
                            CheckinDate = DateTime.Now,
                            ModalityCode = mappedProduct.SupplierCode,
                            IsangoActivityId = Convert.ToString(mappedProduct.IsangoHotelBedsActivityId),//isango ID
                            ProductMapping = new List<IsangoHBProductMapping> { mappedProduct },
                            TourId = Convert.ToInt32(mappedProduct.HotelBedsActivityCode),
                            CheckoutDate = DateTime.Now.AddDays(criteria.Days2Fetch * criteria.Months2Fetch),
                            IsCalendarDumping = true,
                            SupplierOptionIds = new List<string> { mappedProduct.PrefixServiceCode }
                        };
                        raynaCriteria.NoOfPassengers = new Dictionary<Entities.Enums.PassengerType, int>
                {
                    { Entities.Enums.PassengerType.Adult, mappedProduct.MinAdultCount }
                };
                        raynaCriteria.Ages = new Dictionary<PassengerType, int>
                {
                    { Entities.Enums.PassengerType.Adult, 21 }
                };
                        string request = "";
                        string response = "";
                        var activity = _raynaAdapter.GetActivity(raynaCriteria, criteria.Token, out request, out response);
                        if (activity != null)
                        {
                            activitiesList.Add(activity);
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "RaynaCriteriaService",
                            MethodName = "GetAvailability"
                        };
                        _log.Error(isangoErrorEntity, ex);
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
            var tempServiceDetails = new List<TempHBServiceDetail>();
            var svcMapper = new ServiceMapper();

            foreach (Activity activity in activities)
            {
                try
                {
                    IsangoHBProductMapping mappedProduct = mappedProducts.FirstOrDefault(mp => mp.IsangoHotelBedsActivityId == activity.ID);

                    var filterOptions = mappedProducts?.Where(mp => mp.HotelBedsActivityCode.ToString() == activity.Code)?.Select(x => x.PrefixServiceCode)?.ToList();
                    if (filterOptions != null && filterOptions.Count > 0)
                    {
                        activity?.ProductOptions?.RemoveAll(x => !filterOptions.Contains(x.PrefixServiceCode));
                    }

                    if (mappedProduct != null)
                    {
                        List<TempHBServiceDetail> svcDetailsForActivity = svcMapper.ProcessServiceDetailsWithCostPrice(activity, mappedProduct);
                        if (svcDetailsForActivity != null)
                        {
                            tempServiceDetails.AddRange(svcDetailsForActivity);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
             }
            

            return tempServiceDetails;
        }
    }
}