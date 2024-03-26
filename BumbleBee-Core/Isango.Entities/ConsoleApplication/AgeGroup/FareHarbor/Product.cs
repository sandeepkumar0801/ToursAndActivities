using System;

namespace Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor
{
    public class Product
    {
        public string SupplierName { get; set; }
        public int FactsheetId { get; set; }
        public int ServiceId { get; set; }
        public string UserKey { get; set; }

        public DateTime CheckinDate { get; set; }

        public DateTime CheckoutDate { get; set; }
    }
}