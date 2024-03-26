using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FeefoDownloader
{
  [XmlType(AnonymousType = true)]
  public class FEEDBACKLISTFEEDBACK
  {
    private int fEEDBACKIDField;
    private int cOUNTField;
    private string dATEField;
    private DateTime hREVIEWDATEField;
    private string dESCRIPTIONField;
    private string pRODUCTCODEField;
    private string lINKField;
    private string fACEBOOKSHARELINKField;
    private string[] aDDITIONALITEMSField;
    private int hREVIEWRATINGField;
    private bool hREVIEWRATINGFieldSpecified;
    private string pRODUCTRATINGField;
    private string pRODUCTLATESTField;
    private string sERVICERATINGField;
    private string sERVICELATESTField;
    private string cUSTOMERCOMMENTField;
    private string sHORTCUSTOMERCOMMENTField;
    private string vENDORCOMMENTField;
    private string sHORTVENDORCOMMENTField;
    private List<FEEDBACKLISTFEEDBACKPOST> fURTHERCOMMENTSTHREADField;
    private string rEADMOREURLField;
    private string oRDERREFField;
    private string cUSTOMEREMAILField;
    private string cUSTOMERNAMEField;

    public int FEEDBACKID
    {
      get => this.fEEDBACKIDField;
      set => this.fEEDBACKIDField = value;
    }

    public int COUNT
    {
      get => this.cOUNTField;
      set => this.cOUNTField = value;
    }

    public string DATE
    {
      get => this.dATEField;
      set => this.dATEField = value;
    }

    public DateTime HREVIEWDATE
    {
      get => this.hREVIEWDATEField;
      set => this.hREVIEWDATEField = value;
    }

    public string DESCRIPTION
    {
      get => this.dESCRIPTIONField;
      set => this.dESCRIPTIONField = value;
    }

    public string PRODUCTCODE
    {
      get => this.pRODUCTCODEField;
      set => this.pRODUCTCODEField = value;
    }

    public string LINK
    {
      get => this.lINKField;
      set => this.lINKField = value;
    }

    public string FACEBOOKSHARELINK
    {
      get => this.fACEBOOKSHARELINKField;
      set => this.fACEBOOKSHARELINKField = value;
    }

    [XmlArrayItem("ITEM", IsNullable = false)]
    public string[] ADDITIONALITEMS
    {
      get => this.aDDITIONALITEMSField;
      set => this.aDDITIONALITEMSField = value;
    }

    public int HREVIEWRATING
    {
      get => this.hREVIEWRATINGField;
      set => this.hREVIEWRATINGField = value;
    }

    [XmlIgnore]
    public bool HREVIEWRATINGSpecified
    {
      get => this.hREVIEWRATINGFieldSpecified;
      set => this.hREVIEWRATINGFieldSpecified = value;
    }

    public string PRODUCTRATING
    {
      get => this.pRODUCTRATINGField;
      set => this.pRODUCTRATINGField = value;
    }

    public string PRODUCTLATEST
    {
      get => this.pRODUCTLATESTField;
      set => this.pRODUCTLATESTField = value;
    }

    public string SERVICERATING
    {
      get => this.sERVICERATINGField;
      set => this.sERVICERATINGField = value;
    }

    public string SERVICELATEST
    {
      get => this.sERVICELATESTField;
      set => this.sERVICELATESTField = value;
    }

    public string CUSTOMERCOMMENT
    {
      get => this.cUSTOMERCOMMENTField;
      set => this.cUSTOMERCOMMENTField = value;
    }

    public string SHORTCUSTOMERCOMMENT
    {
      get => this.sHORTCUSTOMERCOMMENTField;
      set => this.sHORTCUSTOMERCOMMENTField = value;
    }

    public string VENDORCOMMENT
    {
      get => this.vENDORCOMMENTField;
      set => this.vENDORCOMMENTField = value;
    }

    public string SHORTVENDORCOMMENT
    {
      get => this.sHORTVENDORCOMMENTField;
      set => this.sHORTVENDORCOMMENTField = value;
    }

    [XmlArrayItem("POST", IsNullable = true)]
    public List<FEEDBACKLISTFEEDBACKPOST> FURTHERCOMMENTSTHREAD
    {
      get => this.fURTHERCOMMENTSTHREADField;
      set => this.fURTHERCOMMENTSTHREADField = value;
    }

    public string READMOREURL
    {
      get => this.rEADMOREURLField;
      set => this.rEADMOREURLField = value;
    }

    public string ORDERREF
    {
      get => this.oRDERREFField;
      set => this.oRDERREFField = value;
    }

    public string CUSTOMEREMAIL
    {
      get => this.cUSTOMEREMAILField;
      set => this.cUSTOMEREMAILField = value;
    }

    public string CUSTOMERNAME
    {
      get => this.cUSTOMERNAMEField;
      set => this.cUSTOMERNAMEField = value;
    }
  }
}
