using Logger.Contract;
using Newtonsoft.Json.Linq;
using ServiceAdapters.Tiqets.Tiqets.Commands.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Util;
//using ConstantsForTiqets= ServiceAdapters.Tiqets.Constants.Constant;
using ServiceAdapters.Tiqets.Constants;
using Isango.Entities.Tiqets;
using ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels;
using static ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels.PackageOrderRequest;
using VisitorsDetail = ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels.VisitorsDetail;
using PackageVariant = ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels.PackageVariant;
using PackageProduct = ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels.PackageProduct;

namespace ServiceAdapters.Tiqets.Tiqets.Commands
{
	public class TiqetsPackageCommandHandler : CommandHandlerBase, ITiqetsPackageCommandHandler
	{
		private string _supportEmail = "support@isango.com";
		private string _supportPhoneNumber = "+4402033551240";
		private string _citySightSeeingDefaultEmailForCreateBooking = "no-reply@city-sightseeing.com";
		private string _citySightSeeingDefaultAffiliateID = "58C11104-34E6-47BA-926D-E89E4242B962";

		public TiqetsPackageCommandHandler(ILogger log) : base(log)
		{
			try
			{
				_supportPhoneNumber = ConfigurationManagerHelper.GetValuefromAppSettings("SupportPhoneNumer");
				_supportEmail = ConfigurationManagerHelper.GetValuefromAppSettings("mailfrom");
				_citySightSeeingDefaultEmailForCreateBooking = ConfigurationManagerHelper.GetValuefromAppSettings("CitySightSeeingDefaultEmailForCreateBooking");
				_citySightSeeingDefaultAffiliateID = ConfigurationManagerHelper.GetValuefromAppSettings("CitySightSeeingAffiliateID");
			}
			catch
			{
				//Ignored //Default values are give above
			}
		}

		protected override object TiqetsBookingApiRequest<T>(T inputContext)
		{
			var bookingRequest = inputContext as BookingRequest;
			var createOrderRequest = bookingRequest?.RequestObject as RootTiqets;
			var languageCode = bookingRequest?.TiquetsLanguageCode;
			//if (!string.IsNullOrWhiteSpace(createOrderRequest?.timeslot))
			//{
			//	createOrderRequest.timeslot = createOrderRequest.timeslot.Substring(0, 5);
			//}
			////createOrderRequest.external_reference = bookingRequest.IsangoBookingReference;
			//if (bookingRequest?.AffiliateId?.ToLower() == _citySightSeeingDefaultAffiliateID?.ToLower())
			//{
			//	if (!string.IsNullOrEmpty(createOrderRequest?.CustomerDetail?.email))
			//	{
			//		createOrderRequest.CustomerDetail.email = _citySightSeeingDefaultEmailForCreateBooking;
			//	}
			//}
			var serializedRequestData = SerializeDeSerializeHelper.SerializeWithContractResolver(createOrderRequest);
			var serializedRequest = string.Empty;
			if (serializedRequestData.Contains("passport_ids") || serializedRequestData.Contains("nationality"))
			{
				var removeEntities = JObject.Parse(serializedRequestData);
				removeEntities.Descendants()
				.OfType<JProperty>()
				.Where(attr => (attr.Name == "passport_ids" && attr.Value.ToString() == "") || (attr.Name == "nationality" && attr.Value.ToString() == ""))
				.ToList() // you should call ToList because you're about to changing the result, which is not possible if it is IEnumerable
				.ForEach(attr => attr.Remove()); // removing unwanted attributes

				serializedRequest = removeEntities.ToString();
			}
			else
			{
				serializedRequest = serializedRequestData;
			}
			var signedPayload = GetSignedPayload(serializedRequest, bookingRequest?.AffiliateId);
			var content = new StringContent(signedPayload, Encoding.UTF8, Constant.ApplicationOrJson);
			var url = FormUrl(languageCode);
			using (var httpClient = AddRequestHeadersAndAddressToApi(bookingRequest?.AffiliateId))
			{
				var result = httpClient.PostAsync(url, content);
				result.Wait();
				return result?.GetAwaiter().GetResult();
			}
		}

		protected override object CreateInputRequest<T>(T inputContext)
		{
			var bookingRequest = inputContext as BookingRequest;
			if (bookingRequest?.RequestObject is TiqetsSelectedProduct selectedProduct)
			{
				if (selectedProduct.TimeSlot == "00:00:00")
				{
					selectedProduct.TimeSlot = string.Empty;
				}
				var productOption = string.IsNullOrEmpty(selectedProduct.TimeSlot) ? selectedProduct.ProductOptions.First() : selectedProduct.ProductOptions.FirstOrDefault(x => x.StartTime.ToString().Substring(0, 5).Trim() == selectedProduct.TimeSlot.Substring(0, 5).Trim());

				var customer = productOption?.Customers?.FirstOrDefault(x => x.IsLeadCustomer) ??
					productOption?.Customers?.FirstOrDefault() ??
					selectedProduct.ProductOptions.First().Customers.First();

				//var infants = selectedProduct.package_variants.FirstOrDefault(x => x.package_variant_id == 0);
				//selectedProduct.package_variants.Remove(infants); //Remove Infant as we are setting its value explicitly in the check availability call, no need to pass it in Create Order call


                var package_variants = new List<PackageVariant>();

                if (selectedProduct.Variants != null && selectedProduct.Variants.Count() > 0)
                {
                    foreach (var ivariant in (selectedProduct.Variants))
                    {
                        var package_variant = new PackageVariant();
                        package_variant.package_variant_id = ivariant.Id;
                        package_variant.count = ivariant.Count;

                        package_variants.Add(package_variant);
                    }

                }

				var package_Products = new List<PackageProduct>();

				if(bookingRequest.PackageId != null && bookingRequest.PackageId.Count() > 0)
				{
					foreach (var iProduct in bookingRequest.PackageId)
					{
						var package_Product = new PackageProduct();

						package_Product.package_product_id = iProduct.Product_Id.ToString();
						package_Product.day = productOption?.TravelInfo.StartDate.Date.ToString("yyyy-MM-dd");
						package_Product.timeslot = selectedProduct.TimeSlot ?? string.Empty;


						package_Products.Add(package_Product);
					}
				}
				


                var createOrderRequest = new RootTiqets
				{
                    package_details = new PackageDetails
                    {
                        package_id = selectedProduct.FactSheetId,

                        package_variants = package_variants,

						package_products= package_Products,


						//package_products = bookingRequest.PackageId
      //                  .Select(variant => new PackageProduct
      //                  {
      //                      package_product_id = bookingRequest.PackageId.ToString(),
      //                      day = productOption?.TravelInfo.StartDate.Date.ToString("yyyy-MM-dd"),
      //                      timeslot = selectedProduct.TimeSlot ?? string.Empty
      //                  }).ToList()


                    },
                        customer_details = new CustomerDetail
                        {
                        email = _supportEmail,
                        firstname = customer.FirstName,
                        lastname = customer.LastName,
                        phone = _supportPhoneNumber
                        }
                };



                //Start-Required FullName in Some Cases
                var dataVisitorsDetails = FullNameData(selectedProduct, productOption?.Customers);
				//var dataVisitorsDetails = ContractQuestions(selectedProduct);
				createOrderRequest.visitors_details = dataVisitorsDetails;

				//End -Required FullName in Some Cases
				/*
                if(string.IsNullOrWhiteSpace(createOrderRequest.external_reference))
                {
                    createOrderRequest.external_reference = bookingRequest.IsangoBookingReference;
                }
                */
				bookingRequest.RequestObject = createOrderRequest;
				return bookingRequest;
			}

			return null;
		}

		private List<PackageVariant> FullNameData(
			TiqetsSelectedProduct selectedProduct,
			List<Isango.Entities.Customer> customers)
		{

			var visitorsDetailsLst = new List<PackageVariant>();
			var requiresVisitorsDetails = selectedProduct.RequiresVisitorsDetails;
			if (requiresVisitorsDetails != null && requiresVisitorsDetails.Count > 0)
			{
				var checkStringFullName = "full_name";
				//var checkPassportids = "passport_ids";
				//var checkNationality = "nationality";

				var matchedFullName = requiresVisitorsDetails?.FirstOrDefault(stringToCheck => stringToCheck.Contains(checkStringFullName));
				//var matchedPassportIds = requiresVisitorsDetails?.FirstOrDefault(stringToCheck => stringToCheck.Contains(checkPassportids));
				//var matchedNationality = requiresVisitorsDetails?.FirstOrDefault(stringToCheck => stringToCheck.Contains(checkNationality));

				//if full_name macthed
				if (matchedFullName != null /*|| matchedPassportIds != null || matchedNationality != null*/)
				{
					int i = 0;
					var sumofVariantsCount = selectedProduct?.Variants?.Sum(x => x.Count);
					foreach (var itemVariant in selectedProduct.Variants)
					{
						var visitorsDataLst = new List<VisitorsDetail>();
						var dataCount = itemVariant.Count + i;
						//Assign Customer Name
						for (; i < dataCount; i++)
						{
							//if customer match the total count
							if (sumofVariantsCount == customers.Count)
							{
								var visitorsData = new VisitorsDetail();
								var fullName = customers[i]?.FirstName + " " + customers[i]?.LastName;
								//1. Full Name

								visitorsData.full_name = fullName;
								if (fullName.Any(char.IsDigit))
								{
									visitorsData.full_name = customers[0]?.FirstName + " " + customers[0]?.LastName;
								}

								////2.Passport
								//if (matchedPassportIds != null)
								//{
								//	visitorsData.PassportNumber = customers[i]?.PassportNumber;
								//	if (System.String.IsNullOrEmpty(visitorsData.PassportNumber)
								//	   && selectedProduct?.ContractQuestions != null && selectedProduct?.ContractQuestions?.Count > 0)
								//	{
								//		var passportid = selectedProduct?.ContractQuestions?.Where(x => x.Name?.ToLower() == checkPassportids)?.FirstOrDefault();
								//		if (passportid != null)
								//		{
								//			visitorsData.PassportNumber = passportid?.Answer;
								//		}
								//	}
								//}
								////3. Nationality
								//if (matchedNationality != null)
								//{
								//	visitorsData.PassportNationality = customers[i]?.PassportNationality;
								//	if (System.String.IsNullOrEmpty(visitorsData.PassportNationality)
								//		&& selectedProduct?.ContractQuestions != null && selectedProduct?.ContractQuestions.Count > 0)
								//	{
								//		var nationality = selectedProduct?.ContractQuestions?.Where(x => x.Name?.ToLower() == checkNationality)?.FirstOrDefault();
								//		if (nationality != null)
								//		{
								//			visitorsData.PassportNationality = nationality?.Answer;
								//		}
								//	}
								//}

								visitorsDataLst.Add(visitorsData);
							}
							else //if ispaxdetail required =false, then only one customer
							{
								var visitorsData = new VisitorsDetail
								{
                                    full_name = customers[0]?.FirstName + " " + customers[0]?.LastName,
								};
								//if (matchedPassportIds != null)
								//{
								//	visitorsData.PassportNumber = customers[0]?.PassportNumber;
								//}
								//if (matchedNationality != null)
								//{
								//	visitorsData.PassportNationality = customers[0]?.PassportNationality;
								//}
								visitorsDataLst.Add(visitorsData);
							}

						}
						//Assing VariantId + Customer Name
						var visitorsDetails = new PackageVariant
                        {
                            //VariantId = itemVariant.Id,
                            //VisitorsData = visitorsDataLst
                            visitors_details = visitorsDataLst
                        };
						visitorsDetailsLst.Add(visitorsDetails);
					}
				}
			}
			return visitorsDetailsLst;
		}



		private string FormUrl(string languageCode)
		{
			languageCode = new[] { "ca", "cs", "da", "de", "el", "en", "es", "fr", "it", "ja", "ko", "nl", "pl", "pt", "ru", "sv", "zh" }.Contains(languageCode) ? languageCode : "en";

			return $"{UriConstant.CreateOrder}{languageCode}";
		}
	}
}
