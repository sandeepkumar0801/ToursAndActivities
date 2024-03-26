using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;

namespace Isango.Entities.Hotel
{
    public class HotelOption : ProductOption
    {
        public string RuleText { get; set; }

        public string MealPlan { get; set; }

        public string ChildPolicy { get; set; }

        public HotelOccupancyType OccupancyType { get; set; }

        public int ID { get; set; }

        /// <summary>
        /// Supplier side Option ID
        /// </summary>

        public string SupplierOptionId { get; set; }

        /// <summary>
        /// Hotel ID (specific in case of Jacob)
        /// </summary>

        public int JacobHotelId { get; set; }

        /// <summary>
        /// Hotel ID (specific in case of Jacob)
        /// </summary>

        public int IsangoHotelId { get; set; }

        public Contract Contract { get; set; }

        public HotelRoom HotelRoom { get; set; }

        public string AvailToken { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal NetPrice { get; set; }

        public decimal Commission { get; set; }

        public bool IsNonRefundable { get; set; }
    }
}