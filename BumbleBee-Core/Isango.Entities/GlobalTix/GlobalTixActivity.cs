using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.GlobalTix
{
	public class GlobalTixIdentifier
	{
		public int Id { get; set; }
	}

	public class GlobalTixIdentifierWithName : GlobalTixIdentifier
	{
		public string Name { get; set; }
	}

	public class GlobalTixTicketType : GlobalTixIdentifierWithName
	{
		public string CurrencyCode { get; set; }
		public decimal Price { get; set; }
		public PassengerType PaxType { get; set; }
		public int ToAge { get; set; }
        public int FromAge { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal PayableAmount { get; set; }
        public string Currency { get; set; }
        public string MinimumSellingPrice { get; set; }

        public CancellationNotesActLevel CancellationNotesSetting { get; set; }
        //Property to store ticketTypeIds so that we can hit ''
    }

    public class CancellationNotesActLevel
    {
        public string Id { get; set; }

        public bool IsActive { get; set; }

        public string Value { get; set; }

    }

    public class GlobalTixTicketTypeGroup : GlobalTixIdentifierWithName
	{
		public bool? ApplyCapacity { get; set; }
		public string Desc { get; set; }
		public List<GlobalTixIdentifier> Products { get; set; }
	}

	public class GlobalTixActivity : GlobalTixIdentifier
	{
		public GlobalTixIdentifierWithName Country { get; set; }
		public string Code { get; set; }
		public GlobalTixIdentifierWithName City { get; set; }
		public string ImagePath { get; set; }
		public string Desc { get; set; }
		public string OpHours { get; set; }
		public string Title { get; set; }
		public List<GlobalTixTicketTypeGroup> TicketTypeGroups;
		public List<GlobalTixIdentifierWithName> Types;
		public List<GlobalTixTicketType> TicketTypes;

        public string Latitude { get; set; }
        public string Longitude { get; set; }

    }
}
