using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSuitConsole.Constant
{
    public sealed class Constants
    {
        public const string CheckTiqetStatus = "dbo.usp_checkTiqetStatus";
        public const string EmailSubject = "Tiqet Long-term Unavailable Status Change";

        public const string emailBodyTemplate = "EmailTempl";

        public const string SendtiqetStatusEmail = "SendtiqetStatusEmail";
        public const string TiqetError = "Encountered SendtiqetStatusEmail error ";

        public const string CheckPrioStatus = "dbo.usp_checkPrioStatus";

        public const string PrioEmailSubject = "Prio Status Change";

        public const string OFFLINE = "OFFLINE";

        public const string Active = "Active";

        public const string OfflineStatus = "O";

        public const string PrioError = "Encountered SendPrioStatusEmail error ";
        public const string CheckTiqetTempStatus = "dbo.usp_GetStatusTiqetProduct";
        public const string TiqetTemplate = "TiqetTemplate";

        public const string TourCMSTemplate = "TourCMSTemplate";
        public const string RaynaTemplate = "RaynaTemplate";
        public const string GlobalTixV3Template = "GlobalTixV3Template";

        public const string RaynaOptionsTemplate = "RaynaOptionsTemplate";
        public const string GlobalTixV3OptionsTemplate = "GlobalTixV3OptionsTemplate";

        public const string CheckTiqetTempVariantStatus = "dbo.usp_get_TiqetLabelVariantChange";


        public const string CheckTourCMSTempStatus = "dbo.usp_GetStatusTourCMSProduct";
        public const string CheckTourCMSTempPaxStatus = "dbo.usp_get_TourCMSLabelChange";

        public const string CheckRaynaTempStatus = "dbo.usp_GetStatusRaynaProduct";
        public const string CheckRaynaOptionsTempStatus = "dbo.usp_GetStatusRaynaProductOptions";

        public const string CheckGlobaTixV3TempStatus = "dbo.usp_GetStatusGlobalTixProduct";
        public const string CheckGlobaTixV3OptionsTempStatus = "dbo.usp_GetStatusGlobalTixProductOptions";

    }
}
