using Isango.Entities;
using System.Threading.Tasks;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;
using Constant = WebAPI.Constants.Constant;

namespace WebAPI.Mapper
{
    public class SubscribeMapper
    {
        //public NewsLetterResponse MapNewsLetterResponse(Task<string> result)
        //{
        //    var response = new NewsLetterResponse
        //    {
        //        Status = result.Result,
        //        Message = result.Result == Constant.Subscribed ? Constant.SubscribedMessage
        //        : result.Result == Constant.AlreadyExist ? Constant.AlreadyExistMessage
        //        : Constant.ErrorMessage
        //    };
        //    return response;
        //}

        public string MapNewsLetterResponse(Task<string> result)
        {
            return "subscribed";
        }

        public NewsLetterData MapNewsLetterRequest(NewsLetter newsLetterRequest)
        {
            var criteria = new NewsLetterData
            {
                AffiliateId = newsLetterRequest.AffiliateId,
                Name = newsLetterRequest.Name,
                LanguageCode = newsLetterRequest.LanguageCode,
                CustomerOrigin = newsLetterRequest.CustomerOrigin,
                EmailId = newsLetterRequest.EmailId,
            };
            return criteria;
        }
    }
}