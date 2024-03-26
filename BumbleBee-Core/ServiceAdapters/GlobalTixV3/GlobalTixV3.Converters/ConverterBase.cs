using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.GlobalTixV3;
using ServiceAdapters.GlobalTixV3.Constants;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using System.Collections.Generic;
using System.Linq;
using Util;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Converters
{
    public abstract class ConverterBase
    {
        public ConverterBase()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }

        public MethodType Converter { get; set; }

        public abstract object Convert(object objectResult);
        public abstract object Convert(object objectResult, object input);
        public abstract object Convert(object objectResult, object objectResultDetail, object input);

        protected ContractQuestion ConvertQuestionToContractQuestion(Question q, string condition=null)
		{
            //string opts = (q.Options != null && q.Options.Length > 0) ? $" {string.Join("|", q.Options)}" : string.Empty;

            //         return
            //                 new ContractQuestion
            //                 {
            //                     Code = q.Id.ToString(),
            //                     Name = q.Id.ToString(),
            //                     Description = $"{q.QuestionText}",
            //                     IsRequired = (q.IsOptional == false),
            //                     Answer= opts
            //                 };
            return null;
        }

		protected List<ContractQuestion> ConvertQuestionsToContractQuestions(List<Question> questions, string condition=null)
		{
			if (questions == null || questions.Count <= 0)
			{
				return null;
			}

			return questions.Select(q => ConvertQuestionToContractQuestion(q, condition)).ToList();
		}

		protected PricingUnit GetPricingUnitInstance(PassengerType psgrType)
        {
            switch (psgrType)
            {
                case PassengerType.Adult:
                    return new AdultPricingUnit();
                case PassengerType.Child:
                    return new ChildPricingUnit();
                case PassengerType.Infant:
                    return new InfantPricingUnit();
                case PassengerType.Senior:
                    return new SeniorPricingUnit();
                case PassengerType.Youth:
                    return new YouthPricingUnit();
                case PassengerType.Family:
                    return new FamilyPricingUnit();
                case PassengerType.Student:
                    return new StudentPricingUnit();
                case PassengerType.Pax1:
                    return new Pax1PricingUnit();
                case PassengerType.Pax1To2:
                    return new Pax1To2PricingUnit();
                case PassengerType.Pax3To4:
                    return new Pax3To4PricingUnit();
                case PassengerType.Single:
                    return new SinglePricingUnit();
                case PassengerType.Twin:
                    return new TwinPricingUnit();
                case PassengerType.Concession:
                    return new ConcessionPricingUnit();
                case PassengerType.Under30:
                    return new Under30PricingUnit();
                default:
                    return null;
            }
        }

		//protected void SetImagePaths(Activity act, string imagePath)
		//{
		//	if (imagePath != null)
		//	{
		//		act.Images = new List<ProductImage>
		//		{
		//			new ProductImage { ID=0, Name=imagePath, FileName=$"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GlobalTixBaseUrl)}{Constant.URL_Image}?name={imagePath}", ImageType=ImageType.Bigproduct },
		//			new ProductImage { ID=1, Name=imagePath, FileName=$"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GlobalTixBaseUrl)}{Constant.URL_Image}?name={imagePath}{Constant.Image_Suffix_Banner}", ImageType=ImageType.Smallproduct },
		//		};
		//		act.ThumbNailImage = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GlobalTixBaseUrl)}{Constant.URL_Image}?name={imagePath}{Constant.Image_Suffix_Thumb}";
		//	}

		//}
	}
}
