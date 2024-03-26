using Isango.Entities.GoldenTours;
using ServiceAdapters.TourCMS.TourCMS.Entities.ChannelListResponse;
using ServiceAdapters.TourCMS.TourCMS.Entities.ChannelResponse;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface ITourCMSPersistence
    {
        void SaveChannelData(List<ResponseChannelData> channelListResponse);
        void SaveTourData(List<Tour> tourListResponse);

        void SaveTourRateData(List<TourRateResponse> tourRateListResponse);

        void InsertRedemptionData(string RedemptionJsondata);
        List<int> GetTourCmsChannelId();
    }
}