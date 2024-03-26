using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.WirecardPayment;
using System;

namespace ServiceAdapters.WirecardPayment
{
    public interface IWirecardPaymentAdapter
    {
        Tuple<WirecardPaymentResponse, string, string> Charge(Isango.Entities.Payment.Payment payment, string token);

        Tuple<WirecardPaymentResponse, string, string> Charge3D(Isango.Entities.Payment.Payment payment, Booking booking, string token);

        Tuple<WirecardPaymentResponse, string, string> PreAuthorize(Isango.Entities.Payment.Payment payment, string token);

        Tuple<WirecardPaymentResponse, string, string> PreAuthorize3D(Isango.Entities.Payment.Payment payment, Booking booking, string token);

        Tuple<WirecardPaymentResponse, string, string> PreAuthPurchase3D(Isango.Entities.Payment.Payment preAuthPayment, Isango.Entities.Payment.Payment purchasePayment, Booking booking, string token);

        Tuple<WirecardPaymentResponse, string, string> EnrollmentCheck(Booking booking, string token);

        Tuple<WirecardPaymentResponse, string, string> EmiEnrollmentCheck(Booking booking, Installment installment, string token);

        Tuple<WirecardPaymentResponse, string, string> BookBack(Isango.Entities.Payment.Payment payment, string token);

        Tuple<WirecardPaymentResponse, string, string> Rollback(Isango.Entities.Payment.Payment payment, string token);

        Tuple<WirecardPaymentResponse, string, string> CapturePreauthorize(Isango.Entities.Payment.Payment payment, string token);

        Tuple<WirecardPaymentResponse, string, string> ProcessPayment3D(Isango.Entities.Payment.Payment payment,
            Booking booking, string operationFlag, string token);

        Tuple<WirecardPaymentResponse, string, string> CapturePreauthorize3D(
            Isango.Entities.Payment.Payment preAuthPayment, Isango.Entities.Payment.Payment purchasePayment,
            string token);
    }
}