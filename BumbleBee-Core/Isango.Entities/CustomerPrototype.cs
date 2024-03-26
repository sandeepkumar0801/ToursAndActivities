using Isango.Entities.Enums;
using System;

namespace Isango.Entities
{
    public class CustomerPrototype
    {
        public int CustomerPrototypeId { get; set; }
        public PassengerType PassengerType { get; set; }
        public int ServiceOptionId { get; set; }
        public int ServiceId { get; set; }
        public String StartAt { get; set; }
        public int AgeGroupId { get; set; }
        public bool IsUnitPrice { get; set; }

        /// <summary>
        /// Minimum number of passenger required to booking the unit if IsUnitPrice is true
        /// </summary>
        public int PassengersInUnitMinimum { get; set; }

        /// <summary>
        /// Maximum number of passenger that can be booked in the given unit if IsUnitPrice is true
        /// </summary>
        public int PassengersInUnitMaximum { get; set; }
    }
}