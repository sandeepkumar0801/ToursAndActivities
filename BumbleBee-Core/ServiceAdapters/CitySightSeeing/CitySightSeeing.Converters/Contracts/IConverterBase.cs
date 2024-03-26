using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert<T>(T objectResult, object criteria);

        MethodType Converter { get; set; }
    }
}
