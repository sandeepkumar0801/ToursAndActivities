using Isango.Entities;
using Logger.Contract;
using ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters
{
	public class UpdateBookingConverter : ConverterBase, IUpdateBookingConverter
	{
		public UpdateBookingConverter(ILogger logger) : base(logger)
		{
		}
		public override object Convert(object objectResult)
		{
			return null;
		}

        public SelectedProduct ConvertUpdateBooking(object result)
        {
            return new SelectedProduct();
        }
    }
}