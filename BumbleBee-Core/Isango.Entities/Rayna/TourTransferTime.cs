using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Rayna
{
    public class TourTransferTime
    {
        public int TourId { get; set; }
        public int TourOptionId { get; set; }
        public string TransferType { get; set; }
        public string TransferTime { get; set; }
        public string Duration { get; set; }
        public bool MobileVoucher { get; set; }
        public bool PrintedVoucher { get; set; }
        public bool InstantConfirmation { get; set; }
        public bool OnRequest { get; set; }
        public bool Requiedhrs { get; set; }
    }
}
