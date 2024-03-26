using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceAdapters.Ventrata.Constants;
using ServiceAdapters.Ventrata.Ventrata.Commands.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Entities;

namespace ServiceAdapters.Ventrata.Ventrata.Commands.Contracts
{
    public interface ICommandHandler
    {
        object Execute(InputContext inputContext, string apiLoggingToken);
        object Execute(InputContext inputContext, string apiLoggingToken, out string requestXml,
            out string responseXml);
    }
}
