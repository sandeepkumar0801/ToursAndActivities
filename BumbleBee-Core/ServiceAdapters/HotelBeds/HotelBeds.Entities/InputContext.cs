using Isango.Entities.HotelBeds;
using ServiceAdapters.HotelBeds.Constants;
using System;
using System.Collections.Generic;
using Util;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class InputContext
    {
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Destination { get; set; }

        public string PostUrl { get; set; }

        public string NameSpace { get; set; }

        public MethodType MethodType { get; set; }

        public DateTime CheckinDate { get; set; }

        public DateTime CheckoutDate { get; set; }

        public int Adults { get; set; }

        public int Children { get; set; }

        public List<int> ChildAges { get; set; }

        //TODO: Should it be renamed to SelectedHotel ?
        public List<HotelBedsSelectedProduct> HBSelectedProducts { get; set; }

        public List<int> HotelIDs { get; set; }

        public List<int> FactsheetIDs { get; set; }

        public string Language { get; set; }

        public bool IsPaging { get; set; }
        public int ItemsPerPage { get; set; }

        // For PrioTicket

        public int ActivityId { get; set; }

        public InputContext()
        {
            UserName = ConfigurationManagerHelper.GetValuefromAppSettings("HBuser");
            Password = ConfigurationManagerHelper.GetValuefromAppSettings("HBpassword");
            PostUrl = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.HbServiceUrl);
            NameSpace = ConfigurationManagerHelper.GetValuefromAppSettings("HBXmlNameSpace");
        }

        /// <summary>
        /// Required to filter out right prices from api response.
        /// </summary>
        public object InputCriteria { get; set; }
    }
}