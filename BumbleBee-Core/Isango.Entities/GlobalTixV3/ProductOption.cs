using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.GlobalTixV3
{
    public class ProductOptionV3 
    {
        public bool success { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public string TicketValidity { get; set; }
        public string TicketFormat { get; set; }
        public bool IsFavorited { get; set; }
        public object fromReseller { get; set; }
        public bool? IsCapacity { get; set; }
       public List<string> TimeSlot { get; set; }
        public Visitdate VisitDate { get; set; }
        public List<Question> questions { get; set; }
        public bool? IsCancellable { get; set; }
        public Cancellationpolicy CancellationPolicy { get; set; }
        public List<string> CancellationNotes { get; set; }
        public string Type { get; set; }
        public string DemandType { get; set; }
        public int OptionId { get; set; }
        public List<Tickettype> Tickettype { get; set; }
    }

    public class Visitdate
    {
        public bool request { get; set; }
        public bool required { get; set; }
        public bool isOpenDated { get; set; }
    }

    public class Cancellationpolicy
    {
        public float percentReturn { get; set; }
        public int refundDuration { get; set; }
    }

    public class Question
    {
        public int id { get; set; }
        public List<string> options { get; set; }
        public bool optional { get; set; }
        public string question { get; set; }
        public string type { get; set; }
        public object QuestionCode { get; set; }
        public object IsAnswerLater { get; set; }
    }

    public class Tickettype
    {
        public int ParentProductOptionId { get; set; }
        public int TicketType_Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public float OriginalPrice { get; set; }
        public float OriginalMerchantPrice { get; set; }
        public object IssuanceLimit { get; set; }
        public object MinPurchaseQty { get; set; }
        public object MaxPurchaseQty { get; set; }
        public object MerchantReference { get; set; }
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public bool ApplyToAllQna { get; set; }
        public float NettPrice { get; set; }
        public float NettMerchantPrice { get; set; }
        public float? MinimumSellingPrice { get; set; }
        public float? MinimumMerchantSellingPrice { get; set; }
        public float RecommendedSellingPrice { get; set; }
        public int OptionId { get; set; }

    }
}
