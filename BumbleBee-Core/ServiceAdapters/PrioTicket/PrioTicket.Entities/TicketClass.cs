namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{
    public enum TicketClass
    {
        TicketClassOne = 1,  //Ticket with no timelsots and Always Valid.
        TicketClassTwo = 2,  //Ticket with timeslots and Limited Capacity.
        TicketClassThree = 3   //Ticket with timeslots and Unlimited Capacity.
    }
}