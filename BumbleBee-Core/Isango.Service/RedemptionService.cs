using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.Ventrata;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.TourCMS.TourCMS.Entities.Redemption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Service
{
    public class RedemptionService : IRedemptionService
    {
        private readonly IRedemptionPersistance _redemptionPersistance;
        private readonly ILogger _log;

        public RedemptionService(IRedemptionPersistance redemptionPersistance, ILogger log)
        {
            _redemptionPersistance = redemptionPersistance;
            _log = log;
        }



        public bool VentrataRedemptionService(VentrataRedemption ventrataRedemption)
        {
            try
            {
                var redemdata = new RedemptionData
                {
                    SupplierBookingReferenceNumber = ventrataRedemption.Booking.SupplierReference,
                    IsangoReferenceNumber = ventrataRedemption.Booking.ResellerReference,
                    APIType = (APIType)Enum.Parse(typeof(APIType), "Ventrata"),
                    Status = MapStringToRedemptionStatus(ventrataRedemption.Booking.Status)
                };

                if (redemdata.Status == Redemption_Status.Redemeed)
                {
                    _redemptionPersistance.AddRedemptionDataList(redemdata);
                }

                return true;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedemptionService",
                    MethodName = "VentrataRedemptionService"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        public Redemption_Status MapStringToRedemptionStatus(string statusString)
        {
            if (statusString.Equals("CONFIRMED", StringComparison.OrdinalIgnoreCase))
            {
                return (Redemption_Status)Enum.Parse(typeof(Redemption_Status), "1");
            }
            else if (statusString.Equals("REDEEMED", StringComparison.OrdinalIgnoreCase))
            {
                return (Redemption_Status)Enum.Parse(typeof(Redemption_Status), "2");
            }
            return Redemption_Status.Default;
        }



        public void TourCmsRedemptionService(List<RedemptionBooking> redemptionBookings)
        {
            try
            {
                var redemptionDataList = MapToRedemptionData(redemptionBookings);

                if (redemptionDataList.Any(redemdata => redemdata.Status == Redemption_Status.Redemeed))
                {
                    _redemptionPersistance.AddRedemptionDataList(redemptionDataList.Where(data => data.Status == Redemption_Status.Redemeed).ToList());
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedemptionService",
                    MethodName = "TourCmsRedemptionService"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }



        public List<RedemptionData> MapToRedemptionData(List<RedemptionBooking> bookings)
        {
            var redemptionDataList = new List<RedemptionData>();

            foreach (var booking in bookings)
            {
                bool isRedeemed = booking.BookingComponents.ComponentList.Any(component => component.VoucherRedemptionStatus == 1);

                var redemptionData = new RedemptionData
                {
                    SupplierBookingReferenceNumber = (booking.BookingId).ToString(),
                    IsangoReferenceNumber = booking.AgentRef,
                    APIType = APIType.TourCMS, // Replace with your logic to determine APIType
                    Status = isRedeemed ? Redemption_Status.Redemeed : Redemption_Status.Default
                };

                redemptionDataList.Add(redemptionData);
            }


            return redemptionDataList;
        }


    }
}
