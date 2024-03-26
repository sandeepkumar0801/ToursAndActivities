using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSuitConsole.Models
{
    public class TourCMSProduct
    {
        public int? serviceid { get; set; }
        public int? serviceoptioninserviceid { get; set; }
        public string? SERVICELONGNAME { get; set; }
        public int? AccountId { get; set; }
        public int? ChannelId { get; set; }
        public int? tourid { get; set; }

        public string? NewSaleStatus { get; set; }
        public string? OldSaleStatus { get; set; }
        public string? ProductStatus { get; set; }

    }
}
