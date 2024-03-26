using Isango.Entities;
using Isango.Persistence.Contract;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constant = Isango.Persistence.Constants.Constants;


namespace Isango.Persistence
{
    public class RedemptionPersistance : PersistenceBase, IRedemptionPersistance
    {
        private readonly ILogger _log;
        public RedemptionPersistance(ILogger log)
        {
            _log = log;
        }

        public void AddRedemptionDataList(RedemptionData redemptionData)
        {
            try
            {

                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertRedemptionData))
                {
                    IsangoDataBaseLive.AddInParameter(command, "SupplierBookingReferenceNumber", DbType.String, redemptionData.SupplierBookingReferenceNumber);
                    IsangoDataBaseLive.AddInParameter(command, "IsangoReferenceNumber", DbType.String, redemptionData.IsangoReferenceNumber);

                    IsangoDataBaseLive.AddInParameter(command, "APIType", DbType.Int32, redemptionData.APIType);
                    IsangoDataBaseLive.AddInParameter(command, "Status", DbType.Int32, redemptionData.Status); // Convert the 

                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedemptionPersistance",
                    MethodName = "AddRedemptionDataList",
                    Params = "No parameters here"
                };
                _log.Error(isangoErrorEntity, ex);
            }
        }

        public void AddRedemptionDataList(List<RedemptionData> redemptionDataList)
        {
            try
            {
                foreach (var redemptionData in redemptionDataList)
                {

                    using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertRedemptionData))
                    {
                        IsangoDataBaseLive.AddInParameter(command, "SupplierBookingReferenceNumber", DbType.String, redemptionData.SupplierBookingReferenceNumber);
                        IsangoDataBaseLive.AddInParameter(command, "IsangoReferenceNumber", DbType.String, redemptionData.IsangoReferenceNumber);

                        IsangoDataBaseLive.AddInParameter(command, "APIType", DbType.Int32, redemptionData.APIType);
                        IsangoDataBaseLive.AddInParameter(command, "Status", DbType.Int32, redemptionData.Status); // Convert the 

                        IsangoDataBaseLive.ExecuteNonQuery(command);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedemptionPersistance",
                    MethodName = "AddRedemptionDataList",
                    Params = "No parameters here"
                };
                _log.Error(isangoErrorEntity, ex);
            }
        }


    }
}
