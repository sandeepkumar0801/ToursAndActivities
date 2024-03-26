using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
    public class PaymentExtraDetailsRequest
    {
        [Required]
        public List<PaymentExtraDetail> PaymentExtraDetails { get; set; }

        //[Required]
        public string TokenId { get; set; }
    }

    public class PaymentExtraDetail
    {
        /// <summary>
        /// ReferenceId is the GUID, return by API when you do a check availability call. Path is ActivityId\Options[Index]\BasePrice\PriceAndAvailabilities[Index]\ReferenceId
        /// </summary>
		[Required]
        public string ReferenceId { get; set; }

        //[Required]
        //public TravelInfo TravelInfo { get; set; }
    }
}