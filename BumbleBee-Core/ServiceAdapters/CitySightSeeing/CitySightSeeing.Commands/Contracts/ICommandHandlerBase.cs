using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        object Execute<T>(T inputContext, MethodType methodType, string token);

    }
}
