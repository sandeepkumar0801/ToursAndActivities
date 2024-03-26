using System.Collections.Generic;

namespace Isango.Entities.MoulinRouge
{
    public class MoulinRougeResponse
    {
        public bool MoulinRougeAPICallSuccessful { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Status { get; set; }
        public int BookingId { get; set; }
        public int SupplierId { get; set; }
        public string BookingRefNumber { get; set; }
        public List<MoulinRougeSelectedProduct> SelectedProducts { get; set; }
    }
}