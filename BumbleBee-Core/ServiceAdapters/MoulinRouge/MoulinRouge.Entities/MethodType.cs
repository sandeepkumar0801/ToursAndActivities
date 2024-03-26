namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities
{
    public enum MethodType
    {
        Undefined = 0,
        Catalogdategetlist = 1,
        Catalogdategetdetailmulti = 2,
        Allocseatsautomatic = 3,
        Tempordergetdetail = 4,
        Releaseseats = 5,
        Tempordergetsendingfees = 6,
        Tempordersetsendingfees = 7,
        Createaccount = 8,
        Orderconfirm = 9,
        Getordereticket = 10,

        /// <summary>
        /// Custom Conveter t
        /// To convert resuts from CATALOGDATEGETLIST &  CATALOGDATEGETDETAILMULTI To ActivtyLevel Info
        /// </summary>
        Getdateandprice = 11,

        /// <summary>
        /// To convert resuts from ALLOCSEATSAUTOMATIC to ADDTOBASKET
        /// </summary>
        Addtocart = 12
    }
}