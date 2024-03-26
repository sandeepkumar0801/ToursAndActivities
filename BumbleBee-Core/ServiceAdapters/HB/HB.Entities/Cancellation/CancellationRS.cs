using Newtonsoft.Json;
using System;

namespace ServiceAdapters.HB.HB.Entities.Cancellation
{
    public class CancellationRS : EntityBase
    {
        [JsonProperty("operationId")]
        public string OperationId { get; set; }

        [JsonProperty("auditData")]
        public Auditdata AuditData { get; set; }

        [JsonProperty("booking")]
        public Booking Booking { get; set; }
    }

    public class Auditdata
    {
        [JsonProperty("processTime")]
        public float ProcessTime { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("serverId")]
        public string ServerId { get; set; }

        [JsonProperty("environment")]
        public string Environment { get; set; }
    }

    public class Booking
    {
        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("pendingAmount")]
        public float PendingAmount { get; set; }

        [JsonProperty("agency")]
        public Agency Agency { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("cancellationDate")]
        public DateTime CancellationDate { get; set; }

        [JsonProperty("paymentData")]
        public Paymentdata PaymentData { get; set; }

        [JsonProperty("cancelValuationAmount")]
        public float CancelValuationAmount { get; set; }

        [JsonProperty("clientReference")]
        public string ClientReference { get; set; }

        [JsonProperty("holder")]
        public Holder Holder { get; set; }

        [JsonProperty("total")]
        public float Total { get; set; }

        [JsonProperty("totalNet")]
        public float TotalNet { get; set; }

        [JsonProperty("activities")]
        public Activity[] Activities { get; set; }
    }

    public class Agency
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("branch")]
        public int Branch { get; set; }

        [JsonProperty("comments")]
        public string Comments { get; set; }

        [JsonProperty("sucursal")]
        public Sucursal Sucursal { get; set; }
    }

    public class Sucursal
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }
    }

    public class Paymentdata
    {
        [JsonProperty("paymentType")]
        public Paymenttype PaymentType { get; set; }

        [JsonProperty("invoicingCompany")]
        public Invoicingcompany InvoicingCompany { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class Paymenttype
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public class Invoicingcompany
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("registrationNumber")]
        public string RegistrationNumber { get; set; }
    }

    public class Holder
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mailing")]
        public bool Mailing { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("telephones")]
        public string[] Telephones { get; set; }
    }

    public class Activity
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("supplier")]
        public Supplier Supplier { get; set; }

        [JsonProperty("comments")]
        public Comment[] Comments { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("activityReference")]
        public string ActivityReference { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("modality")]
        public Modality Modality { get; set; }

        [JsonProperty("dateFrom")]
        public string DateFrom { get; set; }

        [JsonProperty("dateTo")]
        public string DateTo { get; set; }

        [JsonProperty("paxes")]
        public Pax[] Paxes { get; set; }

        [JsonProperty("questions")]
        public Question[] Questions { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("agencyCommission")]
        public Agencycommission AgencyCommission { get; set; }

        [JsonProperty("contactInfo")]
        public Contactinfo ContactInfo { get; set; }

        [JsonProperty("amountDetail")]
        public Amountdetail AmountDetail { get; set; }

        [JsonProperty("extraData")]
        public Extradata[] ExtraData { get; set; }

        [JsonProperty("providerInformation")]
        public Providerinformation ProviderInformation { get; set; }
    }

    public class Supplier
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("vatNumber")]
        public string VatNumber { get; set; }
    }

    public class Modality
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rates")]
        public Rate[] Rates { get; set; }

        [JsonProperty("amountUnitType")]
        public string AmountUnitType { get; set; }
    }

    public class Rate
    {
        [JsonProperty("rateDetails")]
        public Ratedetail[] RateDetails { get; set; }
    }

    public class Ratedetail
    {
        [JsonProperty("languages")]
        public Language[] Languages { get; set; }
    }

    public class Language
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public class Agencycommission
    {
        [JsonProperty("percentage")]
        public float Percentage { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("vatAmount")]
        public float VatAmount { get; set; }
    }

    public class Contactinfo
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("country")]
        public Country Country { get; set; }
    }

    public class Country
    {
        [JsonProperty("destinations")]
        public Destination[] Destinations { get; set; }
    }

    public class Destination
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Amountdetail
    {
        [JsonProperty("paxAmounts")]
        public Paxamount[] PaxAmounts { get; set; }

        [JsonProperty("totalAmount")]
        public Totalamount TotalAmount { get; set; }
    }

    public class Totalamount
    {
        [JsonProperty("amount")]
        public float Amount { get; set; }
    }

    public class Paxamount
    {
        [JsonProperty("paxType")]
        public string PaxType { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; }
    }

    public class Providerinformation
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Comment
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class Pax
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mailing")]
        public bool Mailing { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("paxType")]
        public string PaxType { get; set; }

        [JsonProperty("passport")]
        public string Passport { get; set; }
    }

    public class Question
    {
        [JsonProperty("question")]
        public Question1 Question1 { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }
    }

    public class Question1
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }
    }

    public class Extradata
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}