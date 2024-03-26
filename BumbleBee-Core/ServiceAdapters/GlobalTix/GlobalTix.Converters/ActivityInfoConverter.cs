using ServiceAdapters.GlobalTix.GlobalTix.Converters.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;

namespace ServiceAdapters.GlobalTix.GlobalTix.Converters
{
    class ActivityInfoConverter : AbstractActivityConverter, IActivityInfoConverter
    {
        public override object Convert(object objectResult)
        {
            ActivityInfoInputContext actInfoCtx = objectResult as ActivityInfoInputContext;
            if (actInfoCtx == null)
            {
                return null;
            }

            return ConvertInternal(actInfoCtx.ActivityInfo, actInfoCtx.PaxTypeDetails, actInfoCtx.TicketDetails, actInfoCtx);
        }

        public override object Convert(object objectResult, object input)
        {
            return Convert(objectResult);
        }
    }
}
