using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using Isango.Entities.GlobalTix;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities
{
	public class InputContext
	{
        public string AuthToken { get; set; }
        public MethodType MethodType { get; set; }
    }
}
