using Isango.Entities.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
    public class PaxDetail
    {
        /// <summary>
        /// Example Undefined = 0,
        /// Adult = 1,
        /// Child = 2,
        /// Youth = 8,
        /// Infant = 9,
        /// Senior = 10,
        /// Student = 11,
        /// Family = 12,
        /// TwoAndUnder = 13,
        /// Butterbeer = 14,
        /// Concession = 15,
        /// Single = 16,
        /// Twin = 17,
        /// Under30 = 18,
        /// Family2 = 19,
        /// Pax1To2 = 20,
        /// Pax3To4 = 21,
        /// Pax5To6 = 22,
        /// Pax7To8 = 23,
        /// Pax1 = 24,
        /// Pax2 = 25,
        /// Pax3 = 26,
        /// Military = 27,
        /// Family3 = 28
        /// </summary>
        [Required]
        public PassengerType PassengerTypeId { get; set; }

        /// <summary>
        /// No of passengers of given types.
        /// </summary>
        [Required]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Invalid Passenger Count")]
        public int Count { get; set; }

        //Isango team has to remove it after releasing jf2
        //[Required]
        //public int AgeGroupId { get; set; }

        public List<int>? Ages { get; set; }
    }
}