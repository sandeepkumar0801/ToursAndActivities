using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Persistence.Contract
{
    public interface IAdyenPersistence
    {
        Tuple<string, bool> GetAdyenWebhookRepsonse(string bookingRefNo, int transFlowID);


        void UpdateWebhookStatusinDB(int flowName, string bookingReference, string status, string pspReference, string reason, bool isCustomerMailSent, bool isSupplierMailSent, bool? isSuccess);

        Tuple<string, string, string, string,string> UpdatePaymentLinkData(string id, string pspReference);
       
    }
}
