using ActivityWrapper.Entities;
using ActivityWrapper.Helper;
using ActivityWrapper.Mapper;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util;
using Exception = System.Exception;

namespace ActivityWrapper
{
    public class ActivityWrapperService
    {
        private readonly ActivityWrapperHelper _activityHelper;
        private readonly ActivityWrapperMapper _activityMapper;

        public ActivityWrapperService(ActivityWrapperHelper activityHelper, ActivityWrapperMapper activityMapper)
        {
            _activityHelper = activityHelper;
            _activityMapper = activityMapper;
        }

        public IEnumerable<WrapperActivity> GetActivities(List<int> activityIds, ClientInfo clientInfo, Criteria criteria = null)
        {
            try
            {
                var inputCriteria = criteria ?? CreateCriteria();
                var activityList = new List<Activity>();
                clientInfo = _activityHelper.SetClientInfoAffilateInfo(clientInfo);

                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
                Parallel.ForEach(activityIds, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, activityId =>
                {
                    try
                    {
                        activityList.Add(_activityHelper.GetActivity(activityId, clientInfo, inputCriteria));
                    }
                    catch
                    {
                        // ignored
                    }
                });
                return activityList.Count > 0 ? _activityMapper.GetWrapperActivities(activityList) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActivityCalendarAvailabilities GetPriceAndAvailability(int activityId, string affiliateId)
        {
            var activityCalendarAvailabilities = _activityHelper.GetPriceAndAvailabilities(activityId, affiliateId);
            var activityCalendarWithDefaultAffiliateAvailabilities = _activityHelper.GetPriceAndAvailabilities(activityId, "default");
            return _activityMapper.GetPriceAndAvailabilities(activityId, affiliateId, activityCalendarAvailabilities, activityCalendarWithDefaultAffiliateAvailabilities, _activityHelper.IsB2BCategory(affiliateId));
        }

        public Criteria CreateCriteria()
        {
            return new Criteria
            {
                CheckinDate = DateTime.Now,
                CheckoutDate = DateTime.Now,
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Adult, 1 }
                }
            };
        }
    }
}