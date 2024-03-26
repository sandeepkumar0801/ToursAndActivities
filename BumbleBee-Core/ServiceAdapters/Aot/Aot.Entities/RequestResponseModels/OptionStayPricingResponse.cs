using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "Date")]
    public class Date
    {
        [XmlElement(ElementName = "DateFrom")]
        public string DateFrom { get; set; }

        [XmlElement(ElementName = "DateTo")]
        public string DateTo { get; set; }
    }

    [XmlRoot(ElementName = "Dates")]
    public class Dates
    {
        [XmlElement(ElementName = "Date")]
        public Date Date { get; set; }
    }

    [XmlRoot(ElementName = "Policy")]
    public class Policy
    {
        [XmlElement(ElementName = "Dates")]
        public Dates Dates { get; set; }

        [XmlElement(ElementName = "CancelHours")]
        public string CancelHours { get; set; }

        [XmlElement(ElementName = "CancelType")]
        public string CancelType { get; set; }

        [XmlElement(ElementName = "CancelAmount")]
        public string CancelAmount { get; set; }
    }

    [XmlRoot(ElementName = "Policies")]
    public class Policies
    {
        [XmlElement(ElementName = "Policy")]
        public List<Policy> Policy { get; set; }
    }

    [XmlRoot(ElementName = "MachineCancelPolicies")]
    public class MachineCancelPolicies
    {
        [XmlElement(ElementName = "Policies")]
        public Policies Policies { get; set; }
    }

    [XmlRoot(ElementName = "OptStayResults")]
    public class OptStayResults
    {
        [XmlElement(ElementName = "Opt")]
        public string Opt { get; set; }

        [XmlElement(ElementName = "Availability")]
        public string Availability { get; set; }

        [XmlElement(ElementName = "Currency")]
        public string Currency { get; set; }

        [XmlElement(ElementName = "TotalPrice")]
        public string TotalPrice { get; set; }

        [XmlElement(ElementName = "RateName")]
        public string RateName { get; set; }

        [XmlElement(ElementName = "RateText")]
        public string RateText { get; set; }

        [XmlElement(ElementName = "MinSCU")]
        public string MinScu { get; set; }

        [XmlElement(ElementName = "Stay")]
        public string Stay { get; set; }

        [XmlElement(ElementName = "Pay")]
        public string Pay { get; set; }

        [XmlElement(ElementName = "CancelHours")]
        public string CancelHours { get; set; }

        [XmlElement(ElementName = "OptionInfo")]
        public OptionInfo OptionInfo { get; set; }

        [XmlElement(ElementName = "ExtrasRates")]
        public ExtrasRates ExtrasRates { get; set; }

        [XmlElement(ElementName = "MachineCancelPolicies")]
        public MachineCancelPolicies MachineCancelPolicies { get; set; }

        [XmlElement(ElementName = "CancellationPolicy")]
        public string CancellationPolicy { get; set; }

        [XmlElement(ElementName = "RoomType")]
        public string RoomType { get; set; }
    }

    [XmlRoot(ElementName = "OptionStayPricingResponse")]
    public class OptionStayPricingResponse
    {
        [XmlElement(ElementName = "OptStayResults")]
        public List<OptStayResults> OptStayResults { get; set; }
    }
}