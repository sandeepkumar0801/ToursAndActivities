using Isango.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableStorageOperations.Models.Booking
{
    public class SupplierBookingReservationResponse : CustomTableEntity
    {
        public string Id { get; set; }
        public int ApiType { get; set; }
        public string Status { get; set; }
        public string BookingReferenceNo { get; set; }
        public string ReservationReferenceId { get; set; }
        public string Token { get; set; }
        public string AvailabilityReferenceId { get; set; }
        public string OptionName { get; set; }
        public int ServiceOptionId { get; set; }
        public int BookedOptionId { get; set; }
        public string ReservationResponse { get; set; }
        public string DistributorReference { get; set; }
    }
}
