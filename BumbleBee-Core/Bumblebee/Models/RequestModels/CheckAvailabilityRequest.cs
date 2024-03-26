using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
    public class CheckAvailabilityRequest
    {
        [RegularExpression(@"^[0-9]\d*$", ErrorMessage = "Invalid ActivityId")]
        public virtual int ActivityId { get; set; }

        [Required]
        public virtual List<PaxDetail> PaxDetails { get; set; }

        
        public virtual string? AffiliateId { get; set; }

        //[Required]
        public virtual string? LanguageCode { get; set; }

        //[Required]
        public virtual string? CurrencyIsoCode { get; set; }

        public virtual string? TokenId { get; set; }

        [Required]
        public virtual DateTime CheckinDate { get; set; }

        [Required]
        public virtual DateTime CheckoutDate { get; set; }

        [JsonIgnore]
        public virtual string? CountryIp { get; set; }

        public virtual bool IsBundle { get; set; }

        public virtual bool IsQrscan { get; set; }
    }
}