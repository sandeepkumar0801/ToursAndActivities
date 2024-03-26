using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "RoomRates")]
    public class RoomRates
    {
        [XmlElement(ElementName = "SingleRate")]
        public string SingleRate { get; set; }

        [XmlElement(ElementName = "DoubleRate")]
        public string DoubleRate { get; set; }

        [XmlElement(ElementName = "TwinRate")]
        public string TwinRate { get; set; }

        [XmlElement(ElementName = "ExtraChildRate")]
        public string ExtraChildRate { get; set; }
    }

    [XmlRoot(ElementName = "AdultRates")]
    public class AdultRates
    {
        [XmlElement(ElementName = "AdultRate")]
        public string AdultRate { get; set; }
    }

    [XmlRoot(ElementName = "PersonRates")]
    public class PersonRates
    {
        [XmlElement(ElementName = "AdultRates")]
        public AdultRates AdultRates { get; set; }

        [XmlElement(ElementName = "ChildRate")]
        public string ChildRate { get; set; }
    }

    [XmlRoot(ElementName = "OptionRates")]
    public class OptionRates
    {
        [XmlElement(ElementName = "OptionRate")]
        public List<string> OptionRate { get; set; }
    }

    [XmlRoot(ElementName = "OptRate")]
    public class OptRate
    {
        [XmlElement(ElementName = "DateFrom")]
        public string DateFrom { get; set; }

        [XmlElement(ElementName = "DateTo")]
        public string DateTo { get; set; }

        [XmlElement(ElementName = "PersonRates")]
        public PersonRates PersonRates { get; set; }

        [XmlElement(ElementName = "OptionRates")]
        public OptionRates OptionRates { get; set; }

        [XmlElement(ElementName = "ExtrasRates")]
        public ExtrasRates ExtrasRates { get; set; }

        [XmlElement(ElementName = "CancelHours")]
        public string CancelHours { get; set; }

        [XmlElement(ElementName = "RoomRates")]
        public RoomRates RoomRates { get; set; }
    }

    [XmlRoot(ElementName = "Rates")]
    public class Rates
    {
        [XmlElement(ElementName = "OptRate")]
        public List<OptRate> OptRate { get; set; }
    }

    [XmlRoot(ElementName = "OptRates")]
    public class OptRates
    {
        [XmlElement(ElementName = "Opt")]
        public string Opt { get; set; }

        [XmlElement(ElementName = "Currency")]
        public string Currency { get; set; }

        [XmlElement(ElementName = "Periods")]
        public string Periods { get; set; }

        [XmlElement(ElementName = "MinSCU")]
        public string MinScu { get; set; }

        [XmlElement(ElementName = "Rates")]
        public Rates Rates { get; set; }

        [XmlElement(ElementName = "CancelHours")]
        public string CancelHours { get; set; }
    }

    /// <summary>
    /// Getbulkpricingavailabilitydetails response class from aot api
    /// </summary>
    [XmlRoot(ElementName = "OptAvail")]
    public class OptAvail
    {
        [XmlElement(ElementName = "Opt")]
        public string Opt { get; set; }

        [XmlElement(ElementName = "Avail")]
        public string Avail { get; set; }

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "OptRates")]
        public OptRates OptRates { get; set; }
    }

    [XmlRoot(ElementName = "OptionAvailResponse")]
    public class OptionAvailResponse
    {
        [XmlElement(ElementName = "OptAvail")]
        public List<OptAvail> OptAvail { get; set; }
    }
}