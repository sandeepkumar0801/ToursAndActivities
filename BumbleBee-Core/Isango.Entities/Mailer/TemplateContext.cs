using Isango.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Isango.Entities.Mailer
{
    public class TemplateContext
    {
        /// <summary>
        /// Specify the template name if mail body should be picked from the template.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Dictonary containing the keys and values for text palceholers in templates (for search-&-Replace).
        /// </summary>
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// The booking ref number in-case of voucher mailing. A PDF attachment will be sent.
        /// </summary>
        public string BookingRef { get; set; }

        /// <summary>
        /// The booking option ID if cancellation voucher needs to be sent in PDF attachment.
        /// </summary>
        public string BookedOptionId { get; set; }

        /// <summary>
        /// If some custom mail headers should be sent instead of the hardcoded headers.
        /// </summary>
        public List<MailHeader> MailContextList { get; set; }

        /// <summary>
        /// Required if mail headers should be grabbed from Kayako.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Required if mail headers should be grabbed from Kayako.
        /// </summary>
        public Guid AffiliateId { get; set; }

        /// <summary>
        /// Required if mail headers should be grabbed from Kayako.
        /// </summary>
        public ActionType MailActionType { get; set; }

        public bool IsNetRateVoucher { get; set; }

        public bool IsOfflineEmail { get; set; }

        /// <summary>
        /// Booking Option ID
        /// </summary>
        public int BookingID { get; set; }
        public int CVPoints { get; set; }
    }
}