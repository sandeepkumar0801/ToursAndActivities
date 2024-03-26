using Isango.Entities;
using System.Collections.Generic;
using WebAPI.Models.ResponseModels.CheckAvailability;

namespace WebAPI.Models.ResponseModels
{
    public class CheckBundleAvailabilityResponse
    {
        public CheckBundleAvailabilityResponse()
        {
            this.Errors = new List<Error>();
        }

        public int ActivityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TokenId { get; set; }
        public List<BundleOption> BundleOptions { get; set; }
        public bool IsPaxDetailRequired { get; set; }
        public List<Error> Errors { get; set; }
    }
}