using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.GlobalTix;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.GlobalTix;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class GlobalTixCriteriaService : IGlobalTixCriteriaService
    {

        private readonly IGlobalTixAdapter _globalTixAdapter;
        private readonly ILogger _log;

        #region Class Constructors
        public GlobalTixCriteriaService(IGlobalTixAdapter gtAdapter, ILogger logger)
        {
            _globalTixAdapter = gtAdapter;
            _log = logger;
        }
        #endregion

        public List<Activity> GetAvailability(ServiceAvailability.Criteria criteria)
        {
            List<Activity> activitiesList = new List<Activity>();
            try
            {
                foreach (IsangoHBProductMapping mappedProduct in criteria.MappedProducts)
                {
                    try
                    {

                        GlobalTixCriteria gtCriteria = new GlobalTixCriteria()
                        {
                            ActivityId = mappedProduct.HotelBedsActivityCode,
                            ServiceOptionID = mappedProduct?.ServiceOptionInServiceid ?? 0,
                            FactSheetId = mappedProduct.FactSheetId,
                            Days2Fetch = (criteria.Days2Fetch * criteria.Months2Fetch),
                            CheckinDate = DateTime.Now
                        };
                        gtCriteria.NoOfPassengers = new Dictionary<Entities.Enums.PassengerType, int>();
                        gtCriteria.NoOfPassengers.Add(Entities.Enums.PassengerType.Adult, mappedProduct.MinAdultCount);
                        //Temp code starts
                        Activity activity = _globalTixAdapter.GetActivityInformation(gtCriteria, criteria.Token, true);

                        if (activity != null)
                        {
                            activity.ID = mappedProduct.IsangoHotelBedsActivityId;
                            activity.Id = mappedProduct.IsangoHotelBedsActivityId.ToString();

                            var matchedOptions = criteria?.MappedProducts?.Where(x => x.IsangoHotelBedsActivityId == activity.ID);
                            //match with isango data
                            foreach (IsangoHBProductMapping mappedProductItem in matchedOptions)
                            {
                                try
                                {
                                    var databaseServiceCode = mappedProductItem.HotelBedsActivityCode;
                                    var isangoData = activity.ProductOptions?.Where(x => ((ActivityOption)x).Code == databaseServiceCode)?.FirstOrDefault();
                                    if (isangoData != null)
                                    {
                                        isangoData.Id = mappedProductItem.ServiceOptionInServiceid;
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }

                        //Temp code ends
                        if (activity != null)
                        {
                            activitiesList.Add(activity);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error("GlobalTixCriteriaService|GetAvailability", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("GlobalTixCriteriaService|GetAvailability", ex);
                throw;
            }

            return activitiesList;
        }

        public List<TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts)
        {
            List<TempHBServiceDetail> tempServiceDetails = new List<TempHBServiceDetail>();
            ServiceMapper svcMapper = new ServiceMapper();

            foreach (Activity activity in activities)
            {
                try
                {
                    //activityid of MappedData 
                    var getActivityid = activity?.Id; //same for all options
                                                      //Filter Get All Options of activityid
                    var mappedProductsList = mappedProducts?.Where(x => x.IsangoHotelBedsActivityId.ToString() == getActivityid)?.ToList();
                    if (mappedProductsList != null && mappedProductsList.Count>0)
                    {
                        //Filter All Options ids of activityid
                        var lstofOptionsId = mappedProductsList?.Select(x => x.HotelBedsActivityCode)?.ToList();
                        //Filter activity  options based on 
                        if (lstofOptionsId != null && lstofOptionsId.Count>0)
                        {
                            activity.ProductOptions.RemoveAll(x => !lstofOptionsId.Contains(((ActivityOption)x).Code));
                        }
                        //activity?.ProductOptions?.ForEach(x => x.Id = x.ServiceOptionId);
                        foreach (var mappedProduct in mappedProductsList)
                        {
                            List<TempHBServiceDetail> svcDetailsForActivity = svcMapper.ProcessServiceDetailsWithBaseAndCostPrices(activity, mappedProduct);
                            if (svcDetailsForActivity != null)
                            {
                                tempServiceDetails.AddRange(svcDetailsForActivity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("GlobalTixCriteriaService|GetServiceDetails", ex);
                }
            }

            return tempServiceDetails;
        }

    }
}
