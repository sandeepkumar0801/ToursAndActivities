using Microsoft.AspNetCore.Mvc;
using TableStorageOperations.Models.AdditionalPropertiesModels;
using WebAPI.Models.RequestModels;

namespace Bumblebee.Models.RequestModels
{
    public class LoggingData
    {
        public string Request { get; set; }
        public string Oldresponse { get; set; }

        public string CoreResponse { get; set; }
    }
}
