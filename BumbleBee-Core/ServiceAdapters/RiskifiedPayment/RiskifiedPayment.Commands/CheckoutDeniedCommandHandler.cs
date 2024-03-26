using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Enums;
using Isango.Entities.RiskifiedPayment;
using Logger.Contract;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands.Contracts;
using RiskifiedEntities = ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using ClientDetail = ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities.ClientDetail;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands
{
    public class CheckoutDeniedCommandHandler : CommandHandlerBase, ICheckoutDeniedCommandHandler
    {
        public CheckoutDeniedCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest<T>(T inputContext, object requestExtraData, string token)
        {
            var riskifiedCheckout = new RiskifiedEntities.RiskifiedCheckout();
            try
            {
                var booking = inputContext as Booking;
                var authorizationError = requestExtraData as AuthorizationError;
                if (authorizationError == null)
                {
                    authorizationError = new AuthorizationError();
                }
                if (booking != null)
                {
                    var payment = booking.Payment.FirstOrDefault();
                    var creditCard = (CreditCard)payment?.PaymentType;
                    var creditCardHolderName = creditCard.CardHoldersName.Split(new[] { ' ' }, 2);
                    var isGuestLogin = booking.User.IsGuestLogin;
                    var emailVerified = booking.User.IsEmailNBVerified || !isGuestLogin;
                    var clientDetail = booking.ClientDetail;

                    var userAccountType = "";
                    if (!string.IsNullOrEmpty(booking.User.UserLoginSource))
                    {
                        userAccountType = booking.User.UserLoginSource.ToString() == RiskifiedEntities.UserAccountType.facebook.ToString() ? RiskifiedEntities.UserAccountType.facebook.ToString() : RiskifiedEntities.UserAccountType.google.ToString();
                    }
                    else
                    {
                        userAccountType = isGuestLogin ? RiskifiedEntities.UserAccountType.guest.ToString() : RiskifiedEntities.UserAccountType.registered.ToString();
                    }

                    List<RiskifiedEntities.DiscountCode> discountCodes = new List<RiskifiedEntities.DiscountCode>();
                    if (booking.MultisaveAmountOnBooking != 0.0M)
                    {
                        discountCodes.Add(
                            new RiskifiedEntities.DiscountCode()
                            {
                                Amount = (float)booking.MultisaveAmountOnBooking,
                                Code = "MultisaveAmoutOnBooking"
                            }
                        );
                    }
                    RiskifiedEntities.CustomerData riskifiedCustomer;
                    if (userAccountType.ToLower() == "guest")
                    {
                        riskifiedCustomer = new RiskifiedEntities.CustomerData
                        {
                            AccountType = userAccountType,
                            CreatedAt = null
                        };
                    }
                    else
                    {
                        riskifiedCustomer = new RiskifiedEntities.CustomerData
                        {
                            Email = booking.User.EmailAddress,
                            VerifiedEmail = emailVerified,
                            FirstName = booking.User.FirstName,
                            LastName = booking.User.LastName,
                            CreatedAt = booking.User.UserCreationDate,
                            Id = booking.User.EmailAddress,
                            OrdersCount = booking.SelectedProducts.Count,
                            AccountType = userAccountType
                        };
                    }
                    List<RiskifiedEntities.LineItem> lineitems = new List<RiskifiedEntities.LineItem>();
                    List<RiskifiedEntities.Passenger> passengers = new List<RiskifiedEntities.Passenger>();
                    foreach (var product in booking?.IsangoBookingData?.BookedProducts)
                    {
                        if (product.IsPaxDetailRequired)
                        {
                            foreach (var passengerDetail in product?.PassengerDetails)
                            {
                                if (passengers.Any(x => x.FirstName == passengerDetail.FirstName) == false)
                                {
                                    passengers.Add(
                                        new RiskifiedEntities.Passenger()
                                        {
                                            FirstName = passengerDetail.FirstName,
                                            LastName = passengerDetail.LastName,
                                            PassengerType = ((PassengerType)passengerDetail.PassengerTypeId).ToString()
                                        }
                                    );
                                }
                            }
                        }
                        else
                        {
                            var passengerDetail = product?.PassengerDetails?.LastOrDefault();
                            if (passengerDetail != null && passengers.Any(x => x.FirstName == passengerDetail.FirstName) == false)
                            {
                                passengers.Add(
                                    new RiskifiedEntities.Passenger()
                                    {
                                        FirstName = passengerDetail.FirstName,
                                        LastName = passengerDetail.LastName,
                                        PassengerType = ((PassengerType)passengerDetail.PassengerTypeId).ToString()
                                    }
                                );
                            }
                        }
                    }
                    foreach (var selectedProduct in booking?.SelectedProducts?.ToList())
                    {
                        var quantityBooked = booking?.IsangoBookingData?.BookedProducts?.Where(x => x.ServiceId == selectedProduct.Id)?.FirstOrDefault()?.OptionPrice?.PersonCharged ?? 0;
                        var pricingUnits = selectedProduct?.ProductOptions?.FirstOrDefault().SellPrice?.DatePriceAndAvailabilty?
                           .FirstOrDefault().Value?.PricingUnits;
                        Dictionary<decimal, int> pricingData = new Dictionary<decimal, int>();
                        if (pricingUnits?.Count > 0)
                        {
                            pricingData = pricingUnits.GroupBy(x => x.Price).ToDictionary(e => e.Key, e => e.Sum(y => y.Quantity));
                        }
                        var discounts = selectedProduct.AppliedDiscountCoupons;
                        foreach (var discount in discounts)
                        {
                            discountCodes.Add(
                                new RiskifiedEntities.DiscountCode()
                                {
                                    Amount = (float)discount.Price,
                                    Code = discount.Code
                                });
                        }
                        if (pricingData?.Count > 0)
                        {
                            foreach (var priceAndQuantity in pricingData)
                            {
                                if (priceAndQuantity.Key > 0.0M)
                                {
                                    lineitems.Add(
                                        new RiskifiedEntities.LineItem()
                                        {
                                            Price = (float)priceAndQuantity.Key,
                                            RequireShipping = false,
                                            Quantity = priceAndQuantity.Value,
                                            Title = selectedProduct.Name,
                                            ProductId = selectedProduct.Id.ToString(),
                                            Category = "tours",
                                            ProductType = RiskifiedEntities.RiskifiedProductType._event.ToString().Remove(0, 1),
                                            EventDate = selectedProduct?.ProductOptions?.FirstOrDefault()?.TravelInfo?.StartDate ?? default(DateTime),
                                            Section = !string.IsNullOrEmpty(selectedProduct.Category) ? selectedProduct.Category : "tours",
                                            City = selectedProduct.City.Trim(),
                                            CountryCode = selectedProduct.CountryCode,
                                            Latitude = selectedProduct.Latitude,
                                            Longitude = selectedProduct.Longitude
                                        }
                                    );
                                }
                            }
                        }
                        else
                        {
                            lineitems.Add(
                                new RiskifiedEntities.LineItem()
                                {
                                    Price = (float)(selectedProduct.Price / quantityBooked),
                                    RequireShipping = false,
                                    Quantity = quantityBooked,
                                    Title = selectedProduct.Name,
                                    ProductId = selectedProduct.Id.ToString(),
                                    Category = "tours",
                                    ProductType = RiskifiedEntities.RiskifiedProductType._event.ToString().Remove(0, 1),
                                    EventDate = selectedProduct?.ProductOptions?.FirstOrDefault()?.TravelInfo?.StartDate ?? default(DateTime),
                                    Section = !string.IsNullOrEmpty(selectedProduct.Category) ? selectedProduct.Category : "tours",
                                    City = selectedProduct.City.Trim(),
                                    CountryCode = selectedProduct.CountryCode,
                                    Latitude = selectedProduct.Latitude,
                                    Longitude = selectedProduct.Longitude
                                }
                            );
                        }
                    }
                    riskifiedCheckout = new RiskifiedEntities.RiskifiedCheckout()
                    {
                        CheckoutData = new RiskifiedEntities.Checkout()
                        {
                            CheckoutId = booking.ReferenceNumber,
                            Id = booking.ReferenceNumber,
                            Email = booking.User.EmailAddress,
                            CreatedAt = booking.BookingTime,
                            Currency = booking.Currency.IsoCode,
                            Gateway = Convert.ToString(booking.PaymentGateway),
                            BrowserIp = booking.ActualIP,
                            TotalPrice = (float)(booking.Amount + booking.TotalDiscount),
                            //TotalDiscount = (float)(booking.TotalDiscount),
                            ReferringSite = booking.Affiliate.AffiliateCompanyDetail.CompanyWebSite,   // will change this when implementation is complete
                            CartToken = booking.SessionId,
                            VendorName = booking.Affiliate.AffiliateCompanyDetail.CompanyWebSite + "/" + booking.Language.Code,
                            VendorId = booking.Affiliate.Id,
                            DiscountCodes = discountCodes,
                            Customer = riskifiedCustomer,
                            PaymentDetails = new List<RiskifiedEntities.PaymentDetail>()
                            {
                                new RiskifiedEntities.PaymentDetail()
                                {
                                    CreditCardBin = creditCard.CardNumber.Substring(0,6),
                                    CreditCardCompany = creditCard.CardType,
                                    CreditCardNumber = MaskCreditCard(creditCard.CardNumber),
                                    AuthorizationErrorData = new AuthorizationError()
                                    {
                                        CreatedAt = authorizationError.CreatedAt,
                                        ErrorCode = authorizationError.ErrorCode,
                                        Message = authorizationError.Message
                                    }
                                }
                            },
                            Paseengers = passengers,
                            BillingAddress = new RiskifiedEntities.Address()
                            {
                                FirstName = creditCardHolderName[0],
                                LastName = creditCardHolderName.Length > 1 ? creditCardHolderName[1] : "",
                                Address1 = creditCard.CardHoldersAddress1,
                                Address2 = creditCard.CardHoldersAddress2,
                                City = creditCard.CardHoldersCity,
                                Country = creditCard.BillingAddressCountry,
                                CountryCode = creditCard.CardHoldersCountryName,
                                Zip = creditCard.CardHoldersZipCode,
                                Phone = creditCard.CardHoldersPhoneNumber
                            },
                            LineItems = lineitems,
                            Source = clientDetail.IsMobileDevice ? RiskifiedEntities.SourceType.mobile_web.ToString() : RiskifiedEntities.SourceType.desktop_web.ToString(),
                            ClientDetails = new ClientDetail()
                            {
                                AcceptLanguage = clientDetail.AcceptLanguage,
                                UserAgent = clientDetail.UserAgent
                            }
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return riskifiedCheckout;
        }
    }
}