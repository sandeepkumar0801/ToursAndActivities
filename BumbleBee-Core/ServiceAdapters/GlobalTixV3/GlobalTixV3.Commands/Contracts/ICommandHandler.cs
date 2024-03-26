using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands.Contracts
{
    public interface ICommandHandler
    {
        Task<object> ExecuteAsync(InputContext inputContext, string token);
        
        object Execute(InputContext inputContext, string token, out string request, out string response, out HttpStatusCode httpStatusCode);
        object Execute(InputContext inputContext, string token);
       
    }
}
