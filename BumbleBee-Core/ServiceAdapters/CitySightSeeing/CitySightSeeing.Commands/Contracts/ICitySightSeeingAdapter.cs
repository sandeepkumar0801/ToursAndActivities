using Isango.Entities;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands.Contracts
{
    public interface ICitySightSeeingAdapter
    {
        List<ExternalProducts> GetProductExtrnalMappings(string token, string optionExternalId = null);
        string CssBookingResult(string token, CreateBookingRequest bookingRequest);
        ReservationResponse CssReservationResult(string token, ReservationRequest reservationRequest);
        int CancellationResult(string token, CancellationRequest cancellationRequest);
        int RedemptionResult(string token, RedemptionRequest redemptionRequest);
    }
}
