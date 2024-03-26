using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
    public class B2C_CheckAvailabilityRequest : CheckAvailabilityRequest
    {
        /// <summary>
        /// ActivityId/ServiceId. The response will have availability for options of this activityId.
        /// </summary>
        
        [RegularExpression(@"^[0-9]\d*$", ErrorMessage = "Invalid ActivityId")]
        public override int ActivityId { get; set; }

        [RegularExpression(@"^[0-9]\d*$", ErrorMessage = "Invalid ActivityOptionId")]
        public int ActivityOptionId { get; set; }

        /// <summary>
        /// Passenger type Id and its count. The response will have availability as per passed passenger Types
        /// </summary>
        [Required]
        public override List<PaxDetail> PaxDetails { get; set; }

        /// <summary>
        /// Affiliate partner  GUID.
        /// </summary>
        public override string? AffiliateId { get; set; }

        /// <summary>
        /// 2 Char Length ISO language Code. Default is en. Required non English data content in response. Example "en, de, es etc"
        /// </summary>
        public override string? LanguageCode { get; set; }

        /// <summary>
        /// 3 Char Length ISO language Code. Default is en. Required non English data content in response. Example "en, de, es etc"
        /// </summary>
        public override string? CurrencyIsoCode { get; set; }

        public override string? TokenId { get; set; }

        [Required]
        public override DateTime CheckinDate { get; set; }

        [Required]
        public override DateTime CheckoutDate { get; set; }

        [JsonIgnore]
        public override string? CountryIp { get; set; }

        [JsonIgnore]
        public override bool IsBundle { get; set; }

        [JsonIgnore]
        public override bool IsQrscan { get; set; }
    }
}