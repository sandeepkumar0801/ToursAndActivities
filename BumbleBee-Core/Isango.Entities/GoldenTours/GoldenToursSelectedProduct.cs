using System.Collections.Generic;

namespace Isango.Entities.GoldenTours
{
    public class GoldenToursSelectedProduct : SelectedProduct
    {
        #region Request Property

        public Dictionary<int, int> PaxInfo { get; set; }
        public List<ContractQuestion> ContractQuestions { get; set; }

        #endregion Request Property

        #region Response Property

        public string TicketReferenceNumber { get; set; }
        public List<string> QrCodes { get; set; }
        public string TicketUrl { get; set; }

        public string ReferenceNumber { get; set; }

        #endregion Response Property
    }
}