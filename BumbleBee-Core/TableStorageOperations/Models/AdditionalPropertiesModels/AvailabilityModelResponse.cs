using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableStorageOperations.Models.AdditionalPropertiesModels
{
    public class AvailabilityModelResponse : TableEntity
    {
        public string Request { get; set; }
        public string Response { get; set; }
        public string Method { get; set; }
        public string Token { get; set; }
        public string ApiName { get; set; }
        public string PartitionKey1
        {
            get
            {
                return DateTime.UtcNow.ToString("dd_MMM_yyyy");
            }
            set { PartitionKey1 = value; }
        }

    }
}
