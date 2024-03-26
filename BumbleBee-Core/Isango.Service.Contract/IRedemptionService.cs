using Isango.Entities.Ventrata;
using ServiceAdapters.TourCMS.TourCMS.Entities.Redemption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface IRedemptionService
    {
        bool VentrataRedemptionService(VentrataRedemption ventrataRedemption);
        void TourCmsRedemptionService(List<RedemptionBooking> redemptionBookings);
    }
}
