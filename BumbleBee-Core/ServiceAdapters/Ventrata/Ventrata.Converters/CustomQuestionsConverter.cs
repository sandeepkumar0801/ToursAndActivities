using Isango.Entities.Activities;
using Logger.Contract;
using ServiceAdapters.Ventrata.Ventrata.Converters.Contracts;

namespace ServiceAdapters.Ventrata.Ventrata.Converters
{
    public class CustomQuestionsConverter : ConverterBase, ICustomQuestionsConverter
    {
        public CustomQuestionsConverter(ILogger logger) : base(logger)
        {
        }

        public override object Convert(object objectResult, object criteria)
        {
            return ConvertAvailablityResult(objectResult, criteria);
        }

        public override object Convert(object objectResponse)
        {
            throw new NotImplementedException();
        }

        public List<Activity> ConvertAvailablityResult(object optionsFromAPI, object criteria)
        {
            return null;
        }
   }
}