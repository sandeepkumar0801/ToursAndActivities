using System;
namespace Isango.Entities.PrioHub
{
    public class NewPrioProductExtraOptions
    {
        public int ProductId { get; set; }
        public string OptionId { get; set; }
        public string optionName { get; set; }
        public string OptionDescription { get; set; }
        public string OptionType { get; set; }
        public string OptionSelectiontype { get; set; }
        public string OptionCounttype { get; set; }
        public string OptionlistType { get; set; }
        public string OptionpriceType { get; set; }
        public bool OptionMandatory { get; set; }
    }
}