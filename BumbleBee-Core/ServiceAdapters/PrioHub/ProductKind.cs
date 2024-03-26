namespace ServiceAdapters.PrioHub.PrioHub.Entities
{
    public enum ProductKind
    {
        //The kind property serves as a guide to 
        //what type of information this particular object stores.
        location = 0,
        route = 1,
        category = 2,
        product = 3,
        currency = 4,
        tax = 5,
        addon = 6,
        availability = 7,
        reservation = 8,
        order = 9,
        promocode = 10,
        promo = 11,
        webhook = 12,

        notification = 13,
        voucher = 14,
        contact = 15,
        payment = 16,
        credit = 17,
        destination = 18
    }
}