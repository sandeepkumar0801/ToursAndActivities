using Isango.Entities.Enums;

namespace Isango.Entities.Bokun
{
    public class PriceCategory
    {
        public int AgeGroupId { get; set; }
        public int OptionId { get; set; }
        public int ServiceId { get; set; }
        public int PriceCategoryId { get; set; }
        public int ServiceOptionCode { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// Pax type in isango
        /// </summary>
        public PassengerType PassengerTypeId { get; set; }
    }
}