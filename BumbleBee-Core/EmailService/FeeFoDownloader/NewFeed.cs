using System.Collections.Generic;
using System.Xml.Serialization;

namespace FeefoDownloader
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false)]
  public class FEEDBACKLIST
  {
    private FEEDBACKLISTSUMMARY sUMMARYField;
    private List<FEEDBACKLISTFEEDBACK> fEEDBACKField;

    public FEEDBACKLISTSUMMARY SUMMARY
    {
      get => this.sUMMARYField;
      set => this.sUMMARYField = value;
    }

    [XmlElement("FEEDBACK")]
    public List<FEEDBACKLISTFEEDBACK> FEEDBACK
    {
      get => this.fEEDBACKField;
      set => this.fEEDBACKField = value;
    }
  }
}
