using Isango.Entities.Enums;
using System;

namespace Isango.Entities.Activities
{
    public class PaxPrice
    {
        public int ServiceOptionId { get; set; }
        public int PassengerTypeId { get; set; }
        public int PriceId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal GateBasePrice { get; set; }
        public int MinPaxCapacity { get; set; }
        public int MaxPaxCapacity { get; set; }
        public bool ShareablePax { get; set; }
        public DateTime TravelDate { get; set; }
        public PassengerType PassengerType { get; set; }
    }
}