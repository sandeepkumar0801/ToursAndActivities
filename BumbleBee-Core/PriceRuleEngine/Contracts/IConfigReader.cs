using System.Collections.Generic;

namespace PriceRuleEngine.Contracts
{
    public interface IConfigReader
    {
        List<string> GetModuleNames(string category);

        string GetModuleBuilder(string category);
    }
}