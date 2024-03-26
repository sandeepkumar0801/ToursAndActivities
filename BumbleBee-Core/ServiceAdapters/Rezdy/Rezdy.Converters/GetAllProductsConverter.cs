using Isango.Entities.Rezdy;

using ServiceAdapters.Rezdy.Rezdy.Converters.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities.ProductDetails;

using Util;

namespace ServiceAdapters.Rezdy.Rezdy.Converters
{
    public class GetAllProductsConverter : ConverterBase, IGetAllProductsConverter
    {
        public override object Convert(string response)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<GetAllProductResponse>(response.ToString());
            if (result == null) return null;

            return ConvertAllProductResult(result);
        }

        private List<RezdyProduct> ConvertAllProductResult(GetAllProductResponse productResponse)
        {
            var rezdyProducts = new List<RezdyProduct>();

            foreach (var product in productResponse.Products)
            {
                var rezdyProduct = new RezdyProduct
                {
                    ProductType = product.ProductType,
                    AgentPaymentType = product.AgentPaymentType,
                    BookingMode = product.BookingMode,
                    CancellationPolicyDays = product.CancellationPolicyDays,
                    Charter = product.Charter,
                    CommissionIncludesExtras = product.CommissionIncludesExtras,
                    ConfirmMode = product.ConfirmMode,
                    ConfirmModeMinParticipants = product.ConfirmModeMinParticipants,
                    Currency = product.Currency,
                    DateCreated = product.DateCreated,
                    DateUpdated = product.DateUpdated,
                    Description = product.Description,
                    DurationMinutes = product.DurationMinutes,
                    InternalCode = product.InternalCode,
                    MaxCommissionNetRate = product.MaxCommissionNetRate,
                    MaxCommissionPercent = product.MaxCommissionPercent,
                    MinimumNoticeMinutes = product.MinimumNoticeMinutes,
                    Name = product.Name,
                    PickupId = product.PickupId,
                    ProductCode = product.ProductCode,
                    QrCodeType = product.QrCodeType,
                    QuantityRequired = System.Convert.ToBoolean(product.QuantityRequired),
                    QuantityRequiredMax = System.Convert.ToInt32(product.QuantityRequiredMax) == 0 ? 999 : System.Convert.ToInt32(product.QuantityRequiredMax),
                    QuantityRequiredMin = System.Convert.ToInt32(product.QuantityRequiredMin),
                    ShortDescription = product.ShortDescription,
                    SupplierAlias = product.SupplierAlias,
                    SupplierName = product.SupplierName,
                    SupplierId = product.SupplierId,
                    UnitLabel = product.UnitLabel,
                    UnitLabelPlural = product.UnitLabelPlural,
                    WaitListingEnabled = product.WaitListingEnabled,
                    XeroAccount = product.XeroAccount,
                    BookingFields = GetBookingFields(product.BookingFields),
                    PriceOptions = GetPriceOptions(product.PriceOptions),
                    Extras = GetExtras(product.Extras),
                    LocationAddress = GetLocationAddress(product.LocationAddress),
                    Images = GetImages(product.Images),
                    Tags = GetTags(product.Tags)
                };

                rezdyProducts.Add(rezdyProduct);
            }

            return rezdyProducts;
        }

        private Isango.Entities.Rezdy.BookingField[] GetBookingFields(Entities.ProductDetails.BookingField[] responseBookingFields)
        {
            if (responseBookingFields != null)
            {
                var bookingFields = new Isango.Entities.Rezdy.BookingField[responseBookingFields.Length];
                for (var i = 0; i < responseBookingFields.Length; i++)
                {
                    var resBookingFields = responseBookingFields[i];
                    bookingFields[i] = new Isango.Entities.Rezdy.BookingField
                    {
                        FieldType = resBookingFields.FieldType,
                        Label = resBookingFields.Label,
                        ListOptions = resBookingFields.ListOptions,
                        RequiredPerBooking = System.Convert.ToBoolean(resBookingFields.RequiredPerBooking),
                        RequiredPerParticipant = System.Convert.ToBoolean(resBookingFields.RequiredPerParticipant),
                        Value = resBookingFields.Value,
                        VisiblePerBooking = System.Convert.ToBoolean(resBookingFields.VisiblePerBooking),
                        VisiblePerParticipant = System.Convert.ToBoolean(resBookingFields.VisiblePerParticipant)
                    };
                }
                return bookingFields;
            }
            return null;
        }

        private Isango.Entities.Rezdy.PriceOption[] GetPriceOptions(Entities.ProductDetails.PriceOption[] responsePriceOptions)
        {
            if (responsePriceOptions != null)
            {
                var priceOptions = new Isango.Entities.Rezdy.PriceOption[responsePriceOptions.Length];
                for (var i = 0; i < responsePriceOptions.Length; i++)
                {
                    var resPriceOption = responsePriceOptions[i];
                    priceOptions[i] = new Isango.Entities.Rezdy.PriceOption
                    {
                        Id = System.Convert.ToInt32(resPriceOption.Id),
                        Label = resPriceOption.Label,
                        MaxQuantity = System.Convert.ToInt32(resPriceOption.MaxQuantity),
                        MinQuantity = System.Convert.ToInt32(resPriceOption.MinQuantity),
                        Price = resPriceOption.Price,
                        PriceGroupType = resPriceOption.PriceGroupType,
                        ProductCode = resPriceOption.ProductCode,
                        SeatsUsed = System.Convert.ToInt32(resPriceOption.SeatsUsed)
                    };
                }
                return priceOptions;
            }
            return null;
        }

        private Isango.Entities.Rezdy.Extra[] GetExtras(Entities.ProductDetails.Extra[] responseExtra)
        {
            if (responseExtra != null)
            {
                var extras = new Isango.Entities.Rezdy.Extra[responseExtra.Length];
                for (var i = 0; i < responseExtra.Length; i++)
                {
                    var resExtra = responseExtra[i];
                    extras[i] = new Isango.Entities.Rezdy.Extra
                    {
                        Description = resExtra.Description,
                        ExtraPriceType = resExtra.ExtraPriceType,
                        Id = resExtra.Id,
                        Image = new Isango.Entities.Rezdy.Image()
                        {
                            Id = resExtra.Image?.Id,
                            ItemUrl = resExtra.Image?.ItemUrl,
                            LargeSizeUrl = resExtra.Image?.LargeSizeUrl,
                            MediumSizeUrl = resExtra.Image?.MediumSizeUrl,
                            ThumbnailUrl = resExtra.Image?.ThumbnailUrl
                        },
                        Name = resExtra.Name,
                        Price = resExtra.Price,
                        Quantity = resExtra.Quantity
                    };
                }

                return extras;
            }

            return null;
        }

        private Isango.Entities.Rezdy.LocationAddress GetLocationAddress(Entities.ProductDetails.LocationAddress responseLocationAddress)
        {
            if (responseLocationAddress != null)
            {
                var locationAddress = new Isango.Entities.Rezdy.LocationAddress
                {
                    AddressLine = responseLocationAddress.AddressLine,
                    AddressLine2 = responseLocationAddress.AddressLine2,
                    City = responseLocationAddress.City,
                    CountryCode = responseLocationAddress.CountryCode,
                    Latitude = responseLocationAddress.Latitude,
                    Longitude = responseLocationAddress.Longitude,
                    PostCode = responseLocationAddress.PostCode,
                    State = responseLocationAddress.State
                };

                return locationAddress;
            }
            return null;
        }

        private Isango.Entities.Rezdy.Image[] GetImages(Entities.ProductDetails.Image[] responseImages)
        {
            if (responseImages != null)
            {
                var images = new Isango.Entities.Rezdy.Image[responseImages.Length];
                for (var i = 0; i < responseImages.Length; i++)
                {
                    var resImage = responseImages[i];
                    images[i] = new Isango.Entities.Rezdy.Image
                    {
                        Id = resImage.Id,
                        ItemUrl = resImage.ItemUrl,
                        LargeSizeUrl = resImage.LargeSizeUrl,
                        MediumSizeUrl = resImage.MediumSizeUrl,
                        ThumbnailUrl = resImage.ThumbnailUrl
                    };
                }

                return images;
            }

            return null;
        }

        private string[] GetTags(string[] responseTags)
        {
            if (responseTags != null)
            {
                var tags = new string[responseTags.Length];
                for (var i = 0; i < responseTags.Length; i++)
                {
                    var resTag = responseTags[i];

                    tags[i] = resTag;
                }

                return tags;
            }

            return null;
        }
    }
}