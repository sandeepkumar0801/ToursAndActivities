using Newtonsoft.Json.Serialization;
using System.Globalization;

namespace Util
{
    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower(CultureInfo.CurrentCulture);
        }
    }
}