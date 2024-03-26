using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSuitConsole.Models
{
    public class Product
    {
        public int? SERVICEID { get; set; }

        public string? Servicename { get; set; }
        public string? StatusType { get; set; }   
        public string? RegionName { get; set;}
    }
}
