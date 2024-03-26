using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSuitConsole.Models
{
    public class TourCMSPax
    {
        public int? SERVICEID { get; set; }
        public int? serviceoptioninserviceid { get; set; }
        public int? TourId { get; set; }
        public int? AccountId { get; set; }
        public int? ChannelId { get; set; }

        public string? rate_id { get; set; }

        public string? NewLabel { get; set; }

        public string? OldLabel { get; set; }




    }
}
