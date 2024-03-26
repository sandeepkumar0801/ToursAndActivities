using System.Collections.Generic;

namespace ServiceAdapters.HB.HB.Entities
{
    public class InputContext
    {
        public InputContext()
        {
            Filters = new List<Search.Filter>();
            FromDate = string.Empty;
            ToDate = string.Empty;
            Language = string.Empty;
            Pagination = new Search.Pagination();
            Order = string.Empty;

            //ActivityDetail Part
            LstPaxes = new List<ActivityDetail.Pax>();
            ModalityCode = string.Empty;

            //Booking Confirmation
            LstBookingActivities = new List<Booking.Activity>();
            ClientReference = string.Empty;
            Holder = new Booking.Holder();
            LstAnswers = new List<Booking.Answer>();
        }

        public InputContext(List<Search.Filter> lstFilters, string fromDate, string toDate, string language, Search.Pagination paging, string order, List<ActivityDetail.Pax> lstPaxes = null, string modalityCode = null, string activityCode = null, List<Booking.Activity> lstBookingActivities = null, string clientReference = null, Booking.Holder holder = null, List<Booking.Answer> lstAnswers = null)
        {
            Filters = lstFilters;
            FromDate = fromDate;
            ToDate = toDate;
            Language = language;
            Pagination = paging;
            Order = order;

            //ActivityDetail Part
            ActivityCode = !string.IsNullOrEmpty(activityCode) ? activityCode : string.Empty;
            LstPaxes = lstPaxes ?? new List<ActivityDetail.Pax>();
            ModalityCode = !string.IsNullOrEmpty(modalityCode) ? modalityCode : string.Empty;

            //Booking Confirmation
            LstBookingActivities = lstBookingActivities ?? new List<Booking.Activity>();

            if (!string.IsNullOrEmpty(clientReference))
                ClientReference = clientReference;
            else
                ModalityCode = string.Empty;

            Holder = lstBookingActivities == null ? new Booking.Holder() : holder;

            LstAnswers = lstAnswers ?? new List<Booking.Answer>();
        }

        #region SearchRQ input classes

        public List<Search.Filter> Filters { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public string Language { get; set; }

        public Search.Pagination Pagination { get; set; }

        public string Order { get; set; }

        #endregion SearchRQ input classes

        public MethodType MethodType { get; set; }

        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        #region Activity Details input classes

        public string ActivityCode { get; set; }

        public List<ActivityDetail.Pax> LstPaxes { get; set; }

        public string ModalityCode { get; set; }

        #endregion Activity Details input classes

        #region Booking Confirm input classes

        public string BookingReference { get; set; }
        public string ClientReference { get; set; }
        public Booking.Holder Holder { get; set; }
        public List<Booking.Activity> LstBookingActivities { get; set; }
        public List<Booking.Answer> LstAnswers { get; set; }

        #endregion Booking Confirm input classes
    }
}