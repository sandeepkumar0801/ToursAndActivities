using System.Xml.Serialization;

namespace FeefoDownloader
{
  [XmlType(AnonymousType = true)]
  public class FEEDBACKLISTFEEDBACKPOST
  {
    private string dATEField;
    private string cUSTOMERCOMMENTField;
    private string sERVICERATINGField;
    private string pRODUCTRATINGField;
    private string vENDORCOMMENTField;

    public string DATE
    {
      get => this.dATEField;
      set => this.dATEField = value;
    }

    public string CUSTOMERCOMMENT
    {
      get => this.cUSTOMERCOMMENTField;
      set => this.cUSTOMERCOMMENTField = value;
    }

    public string SERVICERATING
    {
      get => this.sERVICERATINGField;
      set => this.sERVICERATINGField = value;
    }

    public string PRODUCTRATING
    {
      get => this.pRODUCTRATINGField;
      set => this.pRODUCTRATINGField = value;
    }

    public string VENDORCOMMENT
    {
      get => this.vENDORCOMMENTField;
      set => this.vENDORCOMMENTField = value;
    }
  }
}
