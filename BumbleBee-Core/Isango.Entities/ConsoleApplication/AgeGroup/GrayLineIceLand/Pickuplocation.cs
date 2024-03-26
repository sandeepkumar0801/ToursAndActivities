using System;

namespace Isango.Entities.ConsoleApplication.AgeGroup.GrayLineIceLand
{
    public class Pickuplocation
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public bool Ticked { get; set; }
        public decimal Price { get; set; }
        public int ProductId { get; set; }
        public int TimePuMinutes { get; set; }
        public bool IsCheckinLocation { get; set; }
        public float Lat { get; set; }
        public float Long { get; set; }
        public string Notes { get; set; }
        public DateTime PickupTime { get; set; }
    }
}