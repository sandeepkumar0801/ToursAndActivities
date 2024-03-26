namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities
{
    public enum SourceType
    {
        desktop_web,
        // Order was placed on the website, using a desktop device

        mobile_web,
        // Order was placed on the mobile website, using a mobile device

        mobile_app,
        // Order was placed on the mobile app, using a mobile device

        web,
        // Order was placed on the website, with no available info about the type of device used

        chat,
        //Order was placed using a chat service

        third_party,
        //Order was placed on a third party domain

        phone,
        // Order was placed over the phone by a call center support or sales agent

        in_store,
        //Order was placed on an in store online device

        shopify_draft_order,
        //Order was a Shopify draft order

        unknown,
        //Order's origin is unknown
    }
}