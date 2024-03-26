using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.Prio;
using Logger.Contract;
using ServiceAdapters.PrioTicket.Constants;
using ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System.Collections.Generic;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters
{
	public class TicketDetailsConverter : ConverterBase, ITicketDetailsConverter
	{
		public TicketDetailsConverter(ILogger logger) : base(logger)
		{
		}
		public override object Convert(object objectResult)
		{
			return ConvertTicketResult(objectResult);
		}

        /// <summary>
        /// Conver Ticket Result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private List<PrioPriceAndAvailability> ConvertTicketResult(object result)
        {
            var ticketDetailRs = (TicketDetailRs)result;
            var priceAndAvailabilities = new List<PrioPriceAndAvailability>();
            var adultSellPrice = new decimal();
            foreach (var ticketTypeDetail in ticketDetailRs.Data.TicketTypeDetails)
            {
                var priceAndAvailability = new PrioPriceAndAvailability
                {
                    AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE
                };
                if (priceAndAvailability.PricingUnits == null)
                {
                    priceAndAvailability.PricingUnits = new List<PricingUnit>();
                }
                if (ticketTypeDetail.TicketType.ToUpper().Contains(Constant.Adult))
                {
                    adultSellPrice = System.Convert.ToDecimal(ticketTypeDetail.UnitListPrice);//Sell Price
                    priceAndAvailability.PricingUnits.Add(new AdultPricingUnit
                    {
                        Price = adultSellPrice
                    });

                    priceAndAvailability.FromDateTime = ticketTypeDetail.StartDate;
                    priceAndAvailability.ToDateTime = ticketTypeDetail.EndDate;
                    priceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                }
                else if (ticketTypeDetail.TicketType.ToUpper().Contains(Constant.Child))
                {
                    priceAndAvailability.PricingUnits.Add(new ChildPricingUnit
                    {
                        Price = System.Convert.ToDecimal(ticketTypeDetail.UnitListPrice)
                    });//Sell Price

                    priceAndAvailability.FromDateTime = ticketTypeDetail.StartDate;
                    priceAndAvailability.ToDateTime = ticketTypeDetail.EndDate;
                    priceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                }
                else if (ticketTypeDetail.TicketType.ToUpper().Contains(Constant.Infant))
                {
                    priceAndAvailability.PricingUnits.Add(new InfantPricingUnit
                    {
                        Price = System.Convert.ToDecimal(ticketTypeDetail.UnitListPrice)
                    });//Sell Price

                    priceAndAvailability.FromDateTime = ticketTypeDetail.StartDate;
                    priceAndAvailability.ToDateTime = ticketTypeDetail.EndDate;
                    priceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                }
                else if (ticketTypeDetail.TicketType.ToUpper().Contains(Constant.Senior))
                {
                    if (System.Convert.ToDecimal(ticketTypeDetail.UnitListPrice) > 0)
                    {
                        priceAndAvailability.PricingUnits.Add(new SeniorPricingUnit
                        {
                            Price = System.Convert.ToDecimal(ticketTypeDetail.UnitListPrice)
                        });//Sell Price

                        priceAndAvailability.FromDateTime = ticketTypeDetail.StartDate;
                        priceAndAvailability.ToDateTime = ticketTypeDetail.EndDate;
                        priceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                    }
                    else //If Senior Price is coming zero in testing API .so, we will make it same as Adult Price.
                    {
                        priceAndAvailability.PricingUnits.Add(new SeniorPricingUnit
                        {
                            Price = adultSellPrice
                        });//Sell Price

                        priceAndAvailability.FromDateTime = ticketTypeDetail.StartDate;
                        priceAndAvailability.ToDateTime = ticketTypeDetail.EndDate;
                        priceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                    }
                }
                else if (ticketTypeDetail.TicketType.ToUpper().Contains(Constant.Family))
                {
                    priceAndAvailability.PricingUnits.Add(new FamilyPricingUnit
                    {
                        Price = System.Convert.ToDecimal(ticketTypeDetail.UnitListPrice)
                    });//Sell Price

                    priceAndAvailability.FromDateTime = ticketTypeDetail.StartDate;
                    priceAndAvailability.ToDateTime = ticketTypeDetail.EndDate;
                    priceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                }
                else if (ticketTypeDetail.TicketType.ToUpper().Contains(Constant.Student))
                {
                    priceAndAvailability.PricingUnits.Add(new StudentPricingUnit
                    {
                        Price = System.Convert.ToDecimal(ticketTypeDetail.UnitListPrice)
                    });//Sell Price

                    priceAndAvailability.FromDateTime = ticketTypeDetail.StartDate;
                    priceAndAvailability.ToDateTime = ticketTypeDetail.EndDate;
                    priceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                }

                priceAndAvailability.TourDepartureId = ticketDetailRs.Data.TicketId;
                priceAndAvailability.Name = ticketDetailRs.Data.TicketTitle;
                priceAndAvailability.TotalPrice = adultSellPrice;
                priceAndAvailabilities.Add(priceAndAvailability);
            }
            return priceAndAvailabilities;
        }
    }
}