using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.GlobalTixV3
{
    public class PackageOptionsV3
    {

        
            
            public object error { get; set; }
            public int size { get; set; }
            public bool success { get; set; }
        
        
            public int id { get; set; }
            public string name { get; set; }
            public string image { get; set; }
            public string description { get; set; }
            public string termsAndConditions { get; set; }
            public string currency { get; set; }
            public DateTime publishStart { get; set; }
            public object publishEnd { get; set; }
            public object redeemStart { get; set; }
            public object redeemEnd { get; set; }
            public string ticketValidity { get; set; }
            public string ticketFormat { get; set; }
            public int definedDuration { get; set; }
            public bool isFavorited { get; set; }
            public object fromReseller { get; set; }
            public string sourceName { get; set; }
            public string sourceTitle { get; set; }
            public bool isAdditionalBookingInfo { get; set; }
            public List<PackageType> packageType { get; set; }
            public List<Inclusion> inclusions { get; set; }
            public object keywords { get; set; }
        

        public class Inclusion
        {
            public int Inclusion_id { get; set; }
            public string Inclusion_name { get; set; }
            public string Inclusion_product { get; set; }
            public int Inclusion_attractionId { get; set; }
        }

        public class PackageType
        {
            public int PackagetypeparentId { get; set; }
            public int PackageType_id { get; set; }
            public string PackageType_sku { get; set; }
            public string PackageType_name { get; set; }
            public double PackageType_nettPrice { get; set; }
            public double PackageType_settlementRate { get; set; }
            public double PackageType_originalPrice { get; set; }
            public object PackageType_issuanceLimit { get; set; }
        }

        


    }
}
