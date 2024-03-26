using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using Util;

namespace ServiceAdapters.GlobalTix.GlobalTix.Converters
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

		protected ContractQuestion ConvertQuestionToContractQuestion(Question q, string condition=null)
		{
			string opts = (q.Options != null && q.Options.Length > 0) ? $" {string.Join("|", q.Options)}" : string.Empty;

            return
                    new ContractQuestion
                    {
                        Code = q.Id.ToString(),
                        Name = q.Id.ToString(),
                        Description = $"{q.QuestionText}",
                        IsRequired = (q.IsOptional == false),
                        Answer= opts
                    };
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
                default:
                    return null;
            }
        }

		protected void SetImagePaths(Activity act, string imagePath)
		{
			if (imagePath != null)
			{
				act.Images = new List<ProductImage>
				{
					new ProductImage { ID=0, Name=imagePath, FileName=$"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GlobalTixBaseUrl)}{Constant.URL_Image}?name={imagePath}", ImageType=ImageType.Bigproduct },
					new ProductImage { ID=1, Name=imagePath, FileName=$"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GlobalTixBaseUrl)}{Constant.URL_Image}?name={imagePath}{Constant.Image_Suffix_Banner}", ImageType=ImageType.Smallproduct },
				};
				act.ThumbNailImage = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GlobalTixBaseUrl)}{Constant.URL_Image}?name={imagePath}{Constant.Image_Suffix_Thumb}";
			}

		}
	}
}
