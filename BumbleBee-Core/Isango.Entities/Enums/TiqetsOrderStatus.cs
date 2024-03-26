namespace Isango.Entities.Enums
{
    public enum TiqetsOrderStatus
    {
        New = 0, //new order, not yet confirmed
        Processing = 1, //confirmed order, still processing
        Failed = 2,     //order failed and can not be fulfilled
        Done = 3,       //order was successful and ticket_pdf_url points to the tickets pdf
        Pending = 4,     //Will take some time to confirm order
        Cancelled = 5
    }
}