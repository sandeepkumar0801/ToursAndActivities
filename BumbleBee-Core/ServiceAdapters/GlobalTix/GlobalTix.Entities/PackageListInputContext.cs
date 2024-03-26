using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities
{
	public class PackageListInputContext : InputContext
	{
		public int PageNumber { get; set; }
	}
}
