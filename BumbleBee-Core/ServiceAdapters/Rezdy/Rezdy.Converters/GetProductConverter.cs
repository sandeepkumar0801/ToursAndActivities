using Isango.Entities.Rezdy;

using ServiceAdapters.Rezdy.Rezdy.Converters.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities.ProductDetails;

using Util;

namespace ServiceAdapters.Rezdy.Rezdy.Converters
{
    public class GetProductConverter : ConverterBase, IGetProductConverter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public override object Convert(string response)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<GetProductResponse>(response.ToString());
            if (result == null || result.RequestStatus.Success == "false") return null;

            return ConvertProductResult(result);
        }

        #region Private Methods

        private RezdyProduct ConvertProductResult(GetProductResponse productResponse)
        {
            var response = productResponse.Product;
            if (response == null)
            {
                return null;
            }
            foreach (var item in response.PriceOptions)
            {
                if (string.IsNullOrEmpty(item.MinQuantity) && string.IsNullOrEmpty(item.MaxQuantity))
                {
                    item.MaxQuantity = response.QuantityRequiredMax;
                    item.MinQuantity = response.QuantityRequiredMin;
                }
            }
            
            var rezdyProduct = new RezdyProduct
            {
                ProductType = response.ProductType,
                AgentPaymentType = response.AgentPaymentType,
                BookingMode = response.BookingMode,
                CancellationPolicyDays = response.CancellationPolicyDays,
                Charter = response.Charter,
                CommissionIncludesExtras = response.CommissionIncludesExtras,
                ConfirmMode = response.ConfirmMode,
                ConfirmModeMinParticipants = response.ConfirmModeMinParticipants,
                Currency = response.Currency,
                DateCreated = response.DateCreated,
                DateUpdated = response.DateUpdated,
                Description = response.Description,
                DurationMinutes = response.DurationMinutes,
                InternalCode = response.InternalCode,
                MaxCommissionNetRate = response.MaxCommissionNetRate,
                MaxCommissionPercent = response.MaxCommissionPercent,
                MinimumNoticeMinutes = response.MinimumNoticeMinutes,
                Name = response.Name,
                PickupId = response.PickupId,
                ProductCode = response.ProductCode,
                QrCodeType = response.QrCodeType,
                QuantityRequired = System.Convert.ToBoolean(response.QuantityRequired),
                QuantityRequiredMax = System.Convert.ToInt32(response.QuantityRequiredMax),
                QuantityRequiredMin = System.Convert.ToInt32(response.QuantityRequiredMin),
                ShortDescription = response.ShortDescription,
                SupplierAlias = response.SupplierAlias,
                SupplierName = response.SupplierName,
                SupplierId = response.SupplierId,
                UnitLabel = response.UnitLabel,
                UnitLabelPlural = response.UnitLabelPlural,
                WaitListingEnabled = response.WaitListingEnabled,
                XeroAccount = response.XeroAccount,
                BookingFields = GetBookingFields(response.BookingFields),
                PriceOptions = GetPriceOptions(response.PriceOptions),
                Extras = GetExtras(response.Extras),
                LocationAddress = GetLocationAddress(response.LocationAddress)
            };

            return rezdyProduct;
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

        #endregion Private Methods
    }
}