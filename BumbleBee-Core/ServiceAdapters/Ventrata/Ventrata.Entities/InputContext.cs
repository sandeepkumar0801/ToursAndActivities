using Isango.Entities.Ventrata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Ventrata.Ventrata.Entities
{
    public class InputContext
    {
        //For Check Availability
        public string SupplierBearerToken { get; set; }
        public string ProductId { get; set; }
        public string OptionCode { get; set; }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public MethodType MethodType { get; set; }
        //public Dictionary<PassengerType, int> PassengerDetails { get; set; }
        public List<PassengerDetails> PassengerDetails { get; set; }

        public string VentrataBaseURL { get; set; }
        public string VentrataIsPerPaxQRCode { get; set; }

        #region Booking Reservation and also Cancellation
        public string pickUpId { get; set; }
        public bool pickUpRequested { get; set; }
        public string AvailabilityId { get; set; }
        public string Uuid { get; set; }
        public List<string> UnitIdsForBooking { get; set; }
        public string ReasonForCancellation { get; set; }

        public List<Isango.Entities.Customer> customers { get; set; }

        public List<VentrataPackages> packages { get; set; }
        #endregion

        #region Booking Confirmation
        public string ResellerReference { get; set; }
        public Contact ContactDetails { get; set; }

        #endregion
    }

    public class Contact {
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNo { get; set; }
    }

    //public class Units {
    //    public string UnitId { get; set; }
    //}
    public class PassengerDetails
    {
        public string PassengerType { get; set; }
        public int Quantity { get; set; }
    }
}
