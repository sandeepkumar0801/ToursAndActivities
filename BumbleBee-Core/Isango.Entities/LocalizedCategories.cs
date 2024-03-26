using System.Collections.Generic;

namespace Isango.Entities
{
    public class LocalizedCategories
    {
        public string Language { get; set; }
        public Dictionary<int, Dictionary<int, string>> DestinationCategories { get; set; }
    }
}