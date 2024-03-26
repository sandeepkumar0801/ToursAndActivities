using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.ConsoleApplication.AgeGroup.Prio
{
    public class ProductDetails
    {
        public int ProductId { get; set; }
        public string Duration { get; set; }
        public string Included { get; set; }
        public string PickUpPoints { get; set; }
        public bool CombiTickets { get; set; }
    }
}
