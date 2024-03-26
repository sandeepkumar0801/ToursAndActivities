using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.HB.HB.Entities.Booking
{
    #region Booking Confirm Request https://api.test.hotelbeds.com/activity-api/3.0/bookings

    /// <summary>
    /// Create booing using this request class
    /// </summary>
    public class BookingConfirmRq
    {
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("clientReference")]
        public string ClientReference { get; set; }

        [JsonProperty("holder")]
        public Holder Holder { get; set; }

        [JsonProperty("activities")]
        public List<Activity> Activities { get; set; }
    }

    /// <summary>
    /// Personal data of the booking holder. If it is also a pax, his or her information is repeated below in the paxes list
    /// </summary>
    public partial class Holder
    {
        /// <summary>
        /// Holder First name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Holder surname
        /// </summary>
        [JsonProperty("surname")]
        public string Surname { get; set; }

        /// <summary>
        /// Holder title (Mr, Ms, Miss)
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Holder e-mail
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Holder postal address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Holder zip code.
        /// </summary>
        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }

        /// <summary>
        /// True If the holder agrees to receive communications via e-mail.
        /// </summary>
        [JsonProperty("mailing")]
        public bool Mailing { get; set; }

        /// <summary>
        /// Holder Country name
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Booking holder telephones list
        /// </summary>
        [JsonProperty("telephones")]
        public List<string> Telephones { get; set; }
    }

    public partial class Activity
    {
        [JsonProperty("preferedLanguage")]
        public string PreferedLanguage { get; set; }

        /// <summary>
        /// Identifies the language in which the activity is going to be offered to the final customer.
        /// </summary>
        [JsonProperty("serviceLanguage")]
        public string ServiceLanguage { get; set; }

        /// <summary>
        /// Identifies the product or service to be booked.
        /// </summary>
        [JsonProperty("rateKey")]
        public string RateKey { get; set; }

        /// <summary>
        /// Identifies the available session selected by the final customer.
        /// </summary>
        ///
        [JsonProperty("session")]
        public string Session { get; set; }

        /// <summary>
        /// From Date
        /// Dates when the final customer is going to attend the activity. If the activity duration is 1 day, then from and to have the same value.
        /// </summary>
        [JsonProperty("from")]
        public string From { get; set; }

        /// <summary>
        /// To Date
        /// Dates when the final customer is going to attend the activity. If the activity duration is 1 day, then from and to have the same value.
        /// </summary>
        [JsonProperty("to")]
        public string To { get; set; }

        /// <summary>
        /// List of paxes and pax type. The ages must match the ones provided in the detail api call. The pax list is optional if there are no questions returned in the “detail” api call that need an answer in the confirmation. If the paxes information is not provided, then a list of paxes with the ages sent in the “detail” api call is returned
        /// </summary>
        [JsonProperty("paxes")]
        public List<Pax> Paxes { get; set; }

        [JsonProperty("answers")]
        public List<Answer> Answers { get; set; }
    }

    /// <summary>
    /// List of paxes and pax type. The ages must match the ones provided in the detail api call. The pax list is optional if there are no questions returned in the “detail” api call that need an answer in the confirmation. If the paxes information is not provided, then a list of paxes with the ages sent in the “detail” api call is returned
    /// </summary>
    public class Pax
    {
        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }
    }

    /// <summary>
    ///  if there are questions returned in the “detail” api call, all question(s) will need an answer in the "confirm" request.
    /// </summary>
    public class Answer
    {
        [JsonProperty("question")]
        public Question Question { get; set; }

        [JsonProperty("answer")]
        public string Answers { get; set; }
    }

    /// <summary>
    /// Question being answered
    /// </summary>
    public partial class Question
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }
    }

    #endregion Booking Confirm Request https://api.test.hotelbeds.com/activity-api/3.0/bookings
}