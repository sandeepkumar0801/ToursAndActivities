namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities
{
    public enum MethodType
    {
        Undefined = 0,
        Decide = 1,
        Decision = 2,
        CheckoutDenied = 3,
        FullRefund = 4,
        PartialRefund = 5,
        FulFill = 5
    }
}