using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.TourCMS
{
   public class TourCMSRedemptionCriteria
    {
        public int ChannelId { get; set; }
        public DateTime CheckinDate { get; set; }

        public DateTime CheckoutDate { get; set; }
        public int page { get; set; }
        public int AccountId { get; set; }

    }
}
