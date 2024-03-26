using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.GlobalTix
{
	public class GlobalTixPackage : GlobalTixIdentifierWithName
	{
		public string Desc { get; set; }
		public PassengerType PaxType { get; set; }
		public int LinkId { get; set; }
		public string CurrencyCode { get; set; }
		public decimal? Price { get; set; }
		public List<int> RelatedPackages { get; set; }
	}
}
