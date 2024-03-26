using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities
{
    public class CssBookingData
    {
        public int adult { get; set; }
        public int child { get; set; }

        public string barcode { get; set; }
        public string booking { get; set; }

        public int resident { get; set; }
        public int senior { get; set; }
        public int infant { get; set; }

        public int student { get; set; }
        public int youth { get; set; }
        public CssCustomer customer { get; set; }

        public string date { get; set; }

        public int BookedOptionId { get; set; }

        public string reservation { get; set; }

        public int CssProductId { get; set; }
        public int CssProductOptionId { get; set; }
        public int SupplierId { get; set; }

        public List<Ticket> tickets { get; set; }

        public int IsangoOptionId { get; set; }

        public string timeslot { get; set; }
        public string OTAReferenceId { get; set; }
        public DateTime utcConfirmedAt { get; set; }


    }
    public class CssCustomer
    {
        public string country { get; set; }
        public string email { get; set; }
        public string full_name { get; set; }
        public string language { get; set; }
        public string lastName { get; set; }
        public string mobile { get; set; }
        public string name { get; set; }

    }
}
