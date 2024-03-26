using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{
	public class ActivityTicketTypeInputContext : InputContext
	{
		 public string TicketType { get; set; }
         public DateTime CheckinDate { get; set; }
         public DateTime CheckOutDate { get; set; }
    }
}
