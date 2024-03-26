namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities
{
    public enum RiskifiedProductType
    {
        /// <summary>
        /// This is a tangible/physical product
        /// </summary>
        physical,

        /// <summary>
        /// This is a digital product (e.g.gift card)
        /// </summary>
        digital,

        /// <summary>
        /// This is a travel industry product (e.g.flight ticket)
        /// </summary>
        travel,

        /// <summary>
        /// This is a travel ride product (e.g.taxi)
        /// </summary>
        ride,

        /// <summary>
        /// This is an event industry product (e.g.concert ticket)
        /// </summary>
        _event,

        /// <summary>
        /// This is an accommodation industry product(e.g.hotel room)
        /// </summary>
        accommodation
    }
}