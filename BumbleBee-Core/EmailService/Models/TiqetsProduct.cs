using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSuitConsole.Models
{
    public class TiqetsProduct
    {
        public int? serviceid { get; set; }

        public string? productid { get; set; }
        public string? NewStatus { get; set; }
        public string? OldStatus { get; set; }
        public string? Sale_Status_Reason { get; set; }
        public string? Sale_Status_Expected_Reopen { get; set; }

        public string? SERVICELONGNAME { get; set; }
        public string? CityName { get; set; }

    }
}
