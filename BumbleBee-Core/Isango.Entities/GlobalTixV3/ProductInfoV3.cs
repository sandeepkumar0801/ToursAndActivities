using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.GlobalTixV3
{
    public class ProductInfoV3
    {   
            public string country { get; set; }
            public double originalPrice { get; set; }
            public List<object> blockedDate { get; set; }
            public float? fromPrice { get; set; }
            public string city { get; set; }
            public string description { get; set; }         
            public int countryId { get; set; } 
            public string currency { get; set; }
            public int id { get; set; }
            public bool isGTRecommend { get; set; }           
            public bool isOpenDated { get; set; }
            public bool isOwnContracted { get; set; }           
            public bool isFavorited { get; set; }
            public bool isBestSeller { get; set; }
            public object fromReseller { get; set; }  
            public string name { get; set; }
            public bool isInstantConfirmation { get; set; }
            public string location { get; set; }
            public string category { get; set; }
            
     

        

    }
}
