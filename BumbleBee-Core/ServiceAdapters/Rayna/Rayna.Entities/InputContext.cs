using Isango.Entities;
using System.Collections.Generic;

namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class InputContext
    {
       
        public SelectedProduct SelectedProducts { get; set; }
        public string BookingReference { get; set; }
        public string VoucherPhoneNumber { get; set; }


        public string UniqueNo { get; set; }
        public string ReferenceNo { get; set; }
        public string ServiceUniqueId { get; set; }
        public int BookingId { get; set; }

        public List<SelectedProduct> SelectedProductsLst { get; set; }

    }
}