namespace Isango.Entities.Hotel
{
    public class Facility
    {
        /// <summary>
        /// Provides ID of the Facility.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Provides the name of the Facility.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Provides the name of the thumbnail image.
        /// </summary>
        public string Image { get; set; }

        public int OrderId { get; set; }

        public bool IsDisplayOnSearch { get; set; }

        public bool IsPaid { get; set; }
    }
}