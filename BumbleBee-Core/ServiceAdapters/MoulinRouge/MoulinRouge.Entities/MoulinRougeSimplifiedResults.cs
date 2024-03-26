using System;
using System.Collections.Generic;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities
{
    /// <summary>
    /// To Create simplified object by combining MoulinRouge Api 2 Calls
    /// ACP_CatalogDateGetList
    /// ACP_CatalogDateGetDetailMulti
    /// </summary>
    public class DateAndPrice
    {
        public MoulinRougeServiceType ServiceType { get; set; }

        /// <summary>
        /// Service type name can of one of the followings based on the rateID and contingentID  combination :-
        ///,"ShowWithDrinks" value="81622:82647"
        ///,"MistinguettMenu" value="81623:82646"
        ///,"VegetarianMenu" value="83664:82646"
        ///,"VeganMenu" value="83665:82646"
        ///,"ToulouseLautrecMenu" value="81624:82646"
        ///,"BelleEpoqueMenu" value="81625:82646"
        ///,"ChristmasDinner" value="81626:82646"
        ///,"ValentineDinner" value="81627:82646"
        /// </summary>
        public string ServiceTypeName { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int CatalogDateId { get; set; }
        public int RateId { get; set; }
        public int ContingentId { get; set; }
        public bool AcceptDemat { get; set; }
        public int CategoryId { get; set; }
        public int FloorId { get; set; }
        public decimal Amount { get; set; }
        public List<decimal> AmountDetail { get; set; }
        public int BlocId { get; set; }
        public int NbMini { get; set; }
        public int Stock { get; set; }
        public string CatalogDateType { get; set; }
        public string Category { get; set; }
        public int ContingentIdRepas { get; set; }
        public int ContingentIdRevue { get; set; }
        public bool LockRepas { get; set; }
        public bool LockRevue { get; set; }
        public string ContingentRepas { get; set; }
        public string ContingentRevue { get; set; }

        /// <summary>
        /// Other Alias :- ServiceID, ProdutID
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        /// Other Alias :- OptionCode RateID+"~"+DateStart.ToString("yyyymmdd HH:00:00").split('~')[1]
        /// </summary>
        public string OptionCode { get; set; }

        /// <summary>
        /// Number of Tickets
        /// </summary>
        public int Quantity { get; set; }
    }

    /// <summary>
    /// To Create simplified object for Allocated seat In MoulinRouge API
    /// ACP_AllocSeatsAutomatic
    /// </summary>
    public class AllocatedSeat
    {
        public string ServiceType { get; set; }

        /// <summary>
        /// Service type name can of one of the followings based on the rateID and contingentID  combination :-
        ///,"ShowWithDrinks" value="81622:82647"
        ///,"MistinguettMenu" value="81623:82646"
        ///,"VegetarianMenu" value="83664:82646"
        ///,"VeganMenu" value="83665:82646"
        ///,"ToulouseLautrecMenu" value="81624:82646"
        ///,"BelleEpoqueMenu" value="81625:82646"
        ///,"ChristmasDinner" value="81626:82646"
        ///,"ValentineDinner" value="81627:82646"
        /// </summary>
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// TempOrderGetDetail
        /// Date time for which booking is done
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// TempOrderGetDetail
        /// Date time for which booking is done
        /// </summary>
        public DateTime DateEnd { get; set; }

        public int CatalogDateId { get; set; }
        public int RateId { get; set; }
        public int ContingentId { get; set; }

        public int CategoryId { get; set; }
        public int FloorId { get; set; }
        public decimal Amount { get; set; }
        public List<decimal> AmountDetail { get; set; }
        public int BlocId { get; set; }

        public string TemporaryOrderId { get; set; }
        public string TemporaryOrderRowId { get; set; }
        public int SeatId { get; set; }
        public bool IsPriceChanged { get; set; }

        public int Quantity { get; set; }

        public bool IsContiguous { get; set; }

        public int IdentityId { get; set; }
        public int RuleId { get; set; }
        public int AccessId { get; set; }
        public int DesignationId { get; set; }
        public int DoorId { get; set; }
        public int PhotoSeatId { get; set; }
        public int PhysicalSeatId { get; set; }

        public int TribuneId { get; set; }
        public int VenueId { get; set; }
        public bool IsNumbered { get; set; }
        public int Rank { get; set; }
        public int Rotation { get; set; }

        /// <summary>
        /// Stock of seats
        /// </summary>
        public int Seat { get; set; }

        public int Status { get; set; }
        public int TypeSeat { get; set; }
        public decimal SeatX { get; set; }
        public decimal SeatY { get; set; }

        /// <summary>
        /// TempOrderGetDetail
        /// To hold CatalogName from TempOrderGetDetail call
        /// </summary>
        public string CatalogName { get; set; }

        /// <summary>
        /// TempOrderGetDetail
        /// it is always 1 in case of isango MoulinRouge calls
        /// We can check CatalogID when we hit TempOrderGetDetail
        /// </summary>
        public int CatalogId { get; set; }
    }

    public class ConfirmedOrder
    {
        public bool IsSuccessful { get; set; }
        public string OrderId { get; set; }
        public string OrderRowId { get; set; }
        public string TemporaryOrderRowId { get; set; }
        public string ETicketGuid { get; set; }
        public decimal Amount { get; set; }
        public string PackageRowId { get; set; }
        public int SeatId { get; set; }
        public string BarCode { get; set; }
        public int FiscalNumber { get; set; }
    }

    /// <summary>
    /// It gives the names of diffrent type of Moulin rouge products
    /// </summary>
    public enum MoulinRougeServiceType
    {
        Undefined = 0,
        Show = 1,
        Dinner = 2
        //    ,
        //SHOW_ShowWithDrinks = 3,
        //DINNER_MistinguettMenu = 4,
        //DINNER_VegetarianMenu = 5,
        //DINNER_VeganMenu = 6,
        //DINNER_ToulouseLautrecMenu = 7,
        //DINNER_BelleEpoqueMenu = 8,
        //DINNER_ChristmasDinner = 9,
        //DINNER_ValentineDinner = 10
    }
}