namespace Isango.Entities
{
    /// <summary>
    /// Specifies the supported transfer option types for an activity
    /// </summary>

    public enum TransferOptionType
    {
        /// <summary>
        /// Undefined value
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Denotes a only arrival option
        /// </summary>
        Arrival = 1,

        /// <summary>
        /// Denotes an only departure option
        /// </summary>
        Departure = 2,

        /// <summary>
        /// Denotes a round trip option
        /// </summary>
        Roundtrip = 3,
    }
}