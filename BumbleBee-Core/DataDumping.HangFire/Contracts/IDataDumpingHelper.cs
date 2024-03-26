using System;
using System.Collections.Generic;

namespace DataDumping.HangFire.Contracts
{
	public interface IDataDumpingHelper
	{
		void LoadGrayLineIceLandData();
	    void LoadMoulinRougeData();
	    void LoadPrioData();
	    void LoadFareHarborData();
	    void LoadBokunData();
	    void LoadAotData();
	    void LoadTiqetsData();
	    void LoadGoldenToursData();
	    void LoadIsangoData();
		void LoadCancellationPolicies();
        void LoadHBApitudeData();
        void LoadRezdyData();
        void LoadRedeamData();
    
        void LoadGlobalTixData();
        void LoadVentrataData();
        void LoadAPIImages();
        List<string> GetAgeDumpingAPIs();

        void LoadTourCMSData();

        void LoadNewCitySightSeeingData();

        void LoadPrioHubData();

        void LoadRaynaData();

        void LoadRedeamV12Data();

        void LoadGlobalTixV3Data();
    }
}