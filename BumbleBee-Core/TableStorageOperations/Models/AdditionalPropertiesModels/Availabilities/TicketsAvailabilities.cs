namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class TicketsAvailabilities : BaseAvailabilitiesEntity
    {
        public string AvailToken { get; set; }
        public string ModalityCode { get; set; }
        public string TicketCode { get; set; }
        public bool IsPaxDetailRequired { get; set; }
        public string Language { get; set; }
        public string IncomingOfficeCode { get; set; }
        public string ContractName { get; set; }
        public string Destination { get; set; }
        public string ContractDetails { get; set; }

        /// <summary>
        /// Required as input to create booking from api
        /// </summary>
        public override string RateKey { get; set; }
    }
}