namespace WebAPI.Models.ResponseModels
{
    public class CurrencyMasterResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string IsoCode { get; set; }
        public string Symbol { get; set; }
        public string ShortSymbol { get; set; }
    }
}