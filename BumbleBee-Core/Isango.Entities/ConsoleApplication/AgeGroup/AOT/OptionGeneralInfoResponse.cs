using System.Collections.Generic;
using System.Xml.Serialization;

namespace Isango.Entities.ConsoleApplication.AgeGroup.AOT
{
    [XmlRoot(ElementName = "OptGeneralInfo")]
    public class OptGeneralInfo
    {
        [XmlElement(ElementName = "Opt")]
        public string Opt { get; set; }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "SupplierCode")]
        public string SupplierCode { get; set; }

        [XmlElement(ElementName = "SupplierName")]
        public string SupplierName { get; set; }

        [XmlElement(ElementName = "Comment")]
        public string Comment { get; set; }

        [XmlElement(ElementName = "BeddingConfiguration")]
        public string BeddingConfiguration { get; set; }

        [XmlElement(ElementName = "MaxPaxDesc")]
        public string MaxPaxDesc { get; set; }

        [XmlElement(ElementName = "LocationID")]
        public string LocationId { get; set; }

        [XmlElement(ElementName = "LocationName")]
        public string LocationName { get; set; }

        [XmlElement(ElementName = "MinChildAge")]
        public string MinChildAge { get; set; }

        [XmlElement(ElementName = "MaxChildAge")]
        public string MaxChildAge { get; set; }

        [XmlElement(ElementName = "ChildPolicyDescription")]
        public string ChildPolicyDescription { get; set; }

        [XmlElement(ElementName = "InfantAgeFrom")]
        public string InfantAgeFrom { get; set; }

        [XmlElement(ElementName = "InfantAgeTo")]
        public string InfantAgeTo { get; set; }

        [XmlElement(ElementName = "ChildAgeFrom")]
        public string ChildAgeFrom { get; set; }

        [XmlElement(ElementName = "ChildAgeTo")]
        public string ChildAgeTo { get; set; }

        [XmlElement(ElementName = "AdultAgeFrom")]
        public string AdultAgeFrom { get; set; }

        [XmlElement(ElementName = "AdultAgeTo")]
        public string AdultAgeTo { get; set; }

        [XmlElement(ElementName = "MaxAdults")]
        public string MaxAdults { get; set; }

        [XmlElement(ElementName = "MaxPax")]
        public string MaxPax { get; set; }

        [XmlElement(ElementName = "Periods")]
        public string Periods { get; set; }

        [XmlElement(ElementName = "SType")]
        public string SType { get; set; }

        [XmlElement(ElementName = "MPFCU")]
        public string Mpfcu { get; set; }

        [XmlElement(ElementName = "SCU")]
        public string Scu { get; set; }

        [XmlElement(ElementName = "MinSCU")]
        public string MinScu { get; set; }

        [XmlElement(ElementName = "MaxSCU")]
        public string MaxScu { get; set; }

        [XmlElement(ElementName = "OptExtras")]
        public OptExtras OptExtras { get; set; }

        [XmlElement(ElementName = "Inclusions")]
        public string Inclusions { get; set; }

        [XmlElement(ElementName = "OptionImportantInfo")]
        public string OptionImportantInfo { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "Class")]
        public string Class { get; set; }

        [XmlElement(ElementName = "images")]
        public Images Images { get; set; }
    }

    [XmlRoot(ElementName = "OptionGeneralInfoResponse")]
    public class OptionGeneralInfoResponse
    {
        [XmlElement(ElementName = "OptGeneralInfo")]
        public List<OptGeneralInfo> OptGeneralInfo { get; set; }
    }
}