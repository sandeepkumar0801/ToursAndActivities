using Isango.Entities;
using Isango.Entities.Ventrata;
using Logger.Contract;
using ServiceAdapters.Ventrata.Ventrata.Converters.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using System.Globalization;
using Util;
using VentrataEntities = ServiceAdapters.Ventrata.Ventrata.Entities;

namespace ServiceAdapters.Ventrata.Ventrata.Converters
{
    public class BookingConfirmationConverter : ConverterBase, IBookingConfirmationConverter
    {
        public BookingConfirmationConverter(ILogger logger) : base(logger)
        {
        }

        public override object Convert(object objectResult)
        {
            var ventrataConfirmedBookingDetails = new VentrataApiBookingDetails();
            var bookingConfirmRes = (BookingConfirmationRes)objectResult;
            var languageCode = "en";
            try
            {
                if (bookingConfirmRes.Status.ToLowerInvariant() == VentrataEntities.VentrataBookingStatus.Confirmed.ToLowerInvariant())
                {
                    //var apiCancellationDetail = bookingConfirmRes.
                    ventrataConfirmedBookingDetails.IsPackage = bookingConfirmRes.IsPackage;
                    ventrataConfirmedBookingDetails.Status = bookingConfirmRes.Status;
                    ventrataConfirmedBookingDetails.OrderId = bookingConfirmRes.OrderId;
                    ventrataConfirmedBookingDetails.ProductId = bookingConfirmRes.ProductId;
                    ventrataConfirmedBookingDetails.OptionIdBooked = bookingConfirmRes.OptionId;
                    ventrataConfirmedBookingDetails.CheckinAvailable = bookingConfirmRes.CheckinAvailable;
                    ventrataConfirmedBookingDetails.CheckedIn = bookingConfirmRes.CheckedIn;
                    ventrataConfirmedBookingDetails.CheckinUrl = bookingConfirmRes.CheckinUrl?.ToString();
                    if (bookingConfirmRes.AvailabilityId != default(DateTime))
                    {
                        ventrataConfirmedBookingDetails.AvailabilityId = (DateTimeOffset.Parse(bookingConfirmRes.AvailabilityId.ToString(), null, DateTimeStyles.RoundtripKind).DateTime).ToString();
                    }
                    ventrataConfirmedBookingDetails.PickUpPoint = bookingConfirmRes.PickupPoint != null ? bookingConfirmRes.PickupPoint.ToString() : string.Empty;

                    ventrataConfirmedBookingDetails.DeliveryMethods = new List<string>();
                    bookingConfirmRes.DeliveryMethods?.ToList().ForEach(thisDelMethod => ventrataConfirmedBookingDetails.DeliveryMethods.Add(thisDelMethod));

                    ventrataConfirmedBookingDetails.ApiCancellationPolicy = bookingConfirmRes?.Cancellation?.ToString();

                    try
                    {
                        if (string.IsNullOrWhiteSpace(ventrataConfirmedBookingDetails?.ApiCancellationPolicy)
                            && bookingConfirmRes?.BookedProduct?.BookedOptions?.Any(x =>
                                                                                        !string.IsNullOrWhiteSpace(x?.cancellationCutoff)
                                                                                    ) == true
                            )
                        {
                            //var cancellationText = string.Empty;
                            //var cutOffDetail = bookingConfirmRes?.BookedProduct?.BookedOptions?.FirstOrDefault(x =>
                            //                                                           !string.IsNullOrWhiteSpace(x?.cancellationCutoff)
                            //                                                        );
                            //var nonRefundableCutOff = "0 hours";
                            //int.TryParse(cutOffDetail?.cancellationCutoff?.ToLower()?.Replace("hours", "")?.Trim(), out var hours);
                            //if (cutOffDetail?.cancellationCutoff?.ToLower() == nonRefundableCutOff || hours == 0)
                            //{
                            //    cancellationText = RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicyNonRefundable);
                            //}
                            //else
                            //{
                            //    cancellationText = $"{string.Format(RESOURCEMANAGER.GetString(languageCode, CONSTANT.CancellationPolicy100ChargableBeforeNhours), hours, hours)}";
                            //}
                            //ventrataConfirmedBookingDetails.ApiCancellationPolicy = cancellationText;

                            ventrataConfirmedBookingDetails.ApiCancellationPolicy = SerializeDeSerializeHelper.Serialize(bookingConfirmRes?.BookedProduct?.BookedOptions);
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = nameof(BookingConfirmationConverter),
                            MethodName = nameof(Convert) + " Process cancellation",
                            Token = "",
                            Params = $"{SerializeDeSerializeHelper.Serialize(bookingConfirmRes?.BookedProduct?.BookedOptions)}"
                        };
                    }
                    ventrataConfirmedBookingDetails.VoucherAtBookingLevel = new BookingLevelVoucher()
                    {
                        DeliveryOptions = new List<BookingLevelVoucherDeliveryoption>(),
                        RedemptionMethod = bookingConfirmRes.Voucher?.RedemptionMethod
                    };

                    //Set booking level voucher/QR Code details and delivery options.
                    bookingConfirmRes.Voucher?.DeliveryOptions?.ToList()?.ForEach(thisDeliveryOption =>
                    {
                        BookingLevelVoucherDeliveryoption optionDelivery = new BookingLevelVoucherDeliveryoption();
                        optionDelivery.DeliveryFormat = thisDeliveryOption.DeliveryFormat;
                        optionDelivery.DeliveryValue = thisDeliveryOption.DeliveryValue;

                        ventrataConfirmedBookingDetails.VoucherAtBookingLevel.DeliveryOptions.Add(optionDelivery);
                    });

                    //Set unit/pax level voucher/QrCode details and delivery options.
                    ventrataConfirmedBookingDetails.UnitItems = new List<UnititemInBookedProduct>();
                    foreach (var unitItem in bookingConfirmRes.UnitItems)
                    {
                        UnititemInBookedProduct unitItemToCreate = new UnititemInBookedProduct();
                        unitItemToCreate.SupplierReference = unitItem.SupplierReference;
                        unitItemToCreate.Status = unitItem.Status;
                        unitItemToCreate.UnitId = unitItem.UnitId;
                        unitItemToCreate.Uuid = unitItem.Uuid;
                        try
                        {
                            unitItemToCreate.Unit = new Isango.Entities.Ventrata.Unit()
                            {
                                Id = unitItem?.Unit?.id,
                                Title = unitItem?.Unit?.title,
                                Subtitle = unitItem?.Unit?.subtitle,
                                TitlePlural = unitItem?.Unit?.titlePlural,
                                InternalName = unitItem?.Unit?.internalName,
                                Type = unitItem?.Unit?.type
                            };
                        }
                        catch(Exception)
                        {
                            //ignore
                        }
                        unitItemToCreate.TicketPerUnitItem = new PaxLevelTicket()
                        {
                            DeliveryOptions = new List<PaxLevelDeliveryoption>(),
                            RedemptionMethod = unitItem?.Ticket?.RedemptionMethod
                        };

                        if(unitItem.Ticket?.DeliveryOptions?.ToList()?.Count > 0)
                        {
                            unitItem.Ticket?.DeliveryOptions?.ToList().ForEach(thisDeliveryOption =>
                            {
                                PaxLevelDeliveryoption optionDelivery = new PaxLevelDeliveryoption();
                                optionDelivery.DeliveryFormat = thisDeliveryOption.DeliveryFormat;
                                optionDelivery.DeliveryValue = thisDeliveryOption.DeliveryValue;

                                unitItemToCreate.TicketPerUnitItem.DeliveryOptions.Add(optionDelivery);
                            });
                        }

                        ventrataConfirmedBookingDetails.UnitItems.Add(unitItemToCreate);
                    }
                }
                return ventrataConfirmedBookingDetails;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = nameof(BookingConfirmationConverter),
                    MethodName = nameof(Convert)
                };
                _logger.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public override object Convert(object objectResponse, object criteria)
        {
            throw new NotImplementedException();
        }
    }
}