using System;

namespace Isango.Entities.ConsoleApplication.DataDumping
{
	public class StorageServiceDetail : CustomTableEntity
	{
		public string TicketTypeId { get; set; }
		public DateTime AvailableOn { get; set; }
		public string Currency { get; set; }
		public int MinPax { get; set; }
		public int ActivityId { get; set; }
		public decimal CommissionPercent { get; set; }
		public decimal SellPrice { get; set; }
		public string Status { get; set; }
		public int ServiceOptionId { get; set; }
		public DateTime CreatedOn { get; set; }
		public string StartTime { get; set; }
		public long UnixStartSec { get; set; }
		public string Variant { get; set; }
		public int Capacity { get; set; }
		public string ApiType { get; set; }
		public string DumpingStatus { get; set; }

		#region Pax Type Changes
		public int PassengerTypeId { get; set; }
		public string UnitType { get; set; }
		#endregion Pax Type Changes
	}

	public enum DumpingStatus
	{
		Unchanged,
		Inserted,
		Updated,
		Deleted
	}
}
