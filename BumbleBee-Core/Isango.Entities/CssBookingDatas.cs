using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities
{
    public class CssBookings
    {
        public List<CssBookingDatas> cssBookingDatas { get; set; }

        public List<CssPassengerDetails> cssPassengerDetails { get; set; }

        public List<CssQrCode> cssQrCodes { get; set; }
    }
    public class CssBookingDatas
    {
        public int BOOKEDOPTIONID { get; set; }
        public string LeadPassengerName { get; set; }

        public string PASSENGERLASTNAME { get; set; }
        public string PASSENGERFIRSTNAME { get; set; }
        public string languagecode { get; set; }
        public string voucheremail { get; set; }
        public string bookingreferencenumber { get; set; }
        public string affiliateid { get; set; }
        public DateTime Bookingdate { get; set; }

        public DateTime utcConfirmedAt { get; set; }
        public DateTime Traveldate { get; set; }
        public int serviceoptioninserviceid { get; set; }
        public int CssProductId { get; set; }
        public int CssProductOptionId { get; set; }
        public string optiontimeslot { get; set; }
        public int AvailabilityReferenceId { get; set; }
        public int SupplierId { get; set; }
        public string OTAReferenceId { get; set; }
    }
    public class CssPassengerDetails
    {
        public int BOOKEDOPTIONID { get; set; }
        public int PASSENGERTYPEID { get; set; }
        public int PaxCount { get; set; }
        public int QRCODECOUNT { get; set; }
        public bool IsPerPaxQRCode { get; set; }
    }

    public class CssQrCode
    {
        public int BOOKEDOPTIONID { get; set; }
        public string QRCODE { get; set; }
        public int PASSENGERTYPEID { get; set; }
    }

}
