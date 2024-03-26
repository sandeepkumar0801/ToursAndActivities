using Isango.Entities;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class LandingPersistence : PersistenceBase, ILandingPersistence
    {
        private readonly ILogger _log;
        public LandingPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// This method returns the list of all the destinations and the attractions.
        /// </summary>
        /// <returns></returns>
        public List<LocalizedMerchandising> LoadLocalizedMerchandising()
        {
            var localizedMerchandising = new List<LocalizedMerchandising>();
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetisangoLocaleMerchandising))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var landingData = new LandingData();

                        while (reader.Read())
                        {
                            localizedMerchandising.Add(landingData.LoadLocalizedMerchandisingData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "LandingPersistence",
                    MethodName = "LoadLocalizedMerchandising",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return localizedMerchandising;
        }
    }
}