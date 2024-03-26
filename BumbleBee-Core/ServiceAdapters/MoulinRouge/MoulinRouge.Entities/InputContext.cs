using System;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities
{
    public class InputContext
    {
        public MethodType MethodType { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}