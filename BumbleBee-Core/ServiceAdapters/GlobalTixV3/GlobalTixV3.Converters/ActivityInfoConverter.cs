using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Converters.Contracts;
using System;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Converters
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

            return ConvertInternal(actInfoCtx);
        }

        public override object Convert(object objectResult, object input)
        {
            return Convert(objectResult);
        }
        public override object Convert(object objectResult, object objectDetailResult, object input)
        {
            throw new NotImplementedException();
        }
    }
}
