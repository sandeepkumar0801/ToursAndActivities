using System.Xml.Serialization;

namespace FeefoDownloader
{
  [XmlType(AnonymousType = true)]
  public class FEEDBACKLISTSUMMARY
  {
    private string mODEField;
    private string vENDORLOGONField;
    private object vENDORREFField;
    private int tOTALSERVICECOUNTField;
    private int tOTALPRODUCTCOUNTField;
    private int cOUNTField;
    private string sUPPLIERLOGOField;
    private string tITLEField;
    private int bESTField;
    private int wORSTField;
    private int aVERAGEField;
    private int sTARTField;
    private int lIMITField;
    private int sERVICEEXCELLENTField;
    private int sERVICEGOODField;
    private int sERVICEPOORField;
    private int sERVICEBADField;
    private int pRODUCTEXCELLENTField;
    private int pRODUCTGOODField;
    private int pRODUCTPOORField;
    private int pRODUCTBADField;
    private int tOTALRESPONSESField;
    private string fEEDGENERATIONField;

    public string MODE
    {
      get => this.mODEField;
      set => this.mODEField = value;
    }

    public string VENDORLOGON
    {
      get => this.vENDORLOGONField;
      set => this.vENDORLOGONField = value;
    }

    public object VENDORREF
    {
      get => this.vENDORREFField;
      set => this.vENDORREFField = value;
    }

    public int TOTALSERVICECOUNT
    {
      get => this.tOTALSERVICECOUNTField;
      set => this.tOTALSERVICECOUNTField = value;
    }

    public int TOTALPRODUCTCOUNT
    {
      get => this.tOTALPRODUCTCOUNTField;
      set => this.tOTALPRODUCTCOUNTField = value;
    }

    public int COUNT
    {
      get => this.cOUNTField;
      set => this.cOUNTField = value;
    }

    public string SUPPLIERLOGO
    {
      get => this.sUPPLIERLOGOField;
      set => this.sUPPLIERLOGOField = value;
    }

    public string TITLE
    {
      get => this.tITLEField;
      set => this.tITLEField = value;
    }

    public int BEST
    {
      get => this.bESTField;
      set => this.bESTField = value;
    }

    public int WORST
    {
      get => this.wORSTField;
      set => this.wORSTField = value;
    }

    public int AVERAGE
    {
      get => this.aVERAGEField;
      set => this.aVERAGEField = value;
    }

    public int START
    {
      get => this.sTARTField;
      set => this.sTARTField = value;
    }

    public int LIMIT
    {
      get => this.lIMITField;
      set => this.lIMITField = value;
    }

    public int SERVICEEXCELLENT
    {
      get => this.sERVICEEXCELLENTField;
      set => this.sERVICEEXCELLENTField = value;
    }

    public int SERVICEGOOD
    {
      get => this.sERVICEGOODField;
      set => this.sERVICEGOODField = value;
    }

    public int SERVICEPOOR
    {
      get => this.sERVICEPOORField;
      set => this.sERVICEPOORField = value;
    }

    public int SERVICEBAD
    {
      get => this.sERVICEBADField;
      set => this.sERVICEBADField = value;
    }

    public int PRODUCTEXCELLENT
    {
      get => this.pRODUCTEXCELLENTField;
      set => this.pRODUCTEXCELLENTField = value;
    }

    public int PRODUCTGOOD
    {
      get => this.pRODUCTGOODField;
      set => this.pRODUCTGOODField = value;
    }

    public int PRODUCTPOOR
    {
      get => this.pRODUCTPOORField;
      set => this.pRODUCTPOORField = value;
    }

    public int PRODUCTBAD
    {
      get => this.pRODUCTBADField;
      set => this.pRODUCTBADField = value;
    }

    public int TOTALRESPONSES
    {
      get => this.tOTALRESPONSESField;
      set => this.tOTALRESPONSESField = value;
    }

    public string FEEDGENERATION
    {
      get => this.fEEDGENERATIONField;
      set => this.fEEDGENERATIONField = value;
    }
  }
}
