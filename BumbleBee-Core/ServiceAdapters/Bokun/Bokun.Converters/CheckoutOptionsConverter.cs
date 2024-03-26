using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Converters.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities.CheckoutOptions;
using ServiceAdapters.Bokun.Constants;
using Util;
using BokunEntities = Isango.Entities.Bokun;

namespace ServiceAdapters.Bokun.Bokun.Converters
{
    public class CheckoutOptionsConverter : ConverterBase, ICheckoutOptionsConverter
    {
        public CheckoutOptionsConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult">Generic model for Get Checkout Options Call</param>
        /// <param name="criteria">Generic request model</param>
        /// <returns></returns>
        public object Convert<T>(T objectResult, T criteria)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<CheckoutOptionsRs>(objectResult.ToString());
            if (result == null)
                return (CheckoutOptionsRs)null;
            return ConvertToQuestionResult(result);
        }

        public List<BokunEntities.Question> ConvertToQuestionResult(CheckoutOptionsRs result)
        {
            var questions = new List<BokunEntities.Question>();
            result.Questions.MainContactDetails.ForEach(question =>
                {
                    questions.Add((GetQuestionsFromResponse(question, Constant.MainContactDetails)));
                });

            foreach (var option in result?.Options)
            {
                foreach (var question in option.Questions)
                {
                    if (question.Required && question.QuestionId != Constant.SendNotificationToMainContact)
                        questions.Add(GetQuestionsFromResponse(question, Constant.CustomerFullPayment));
                }
            }
            result.Questions.ActivityBookings?.ForEach(activityBooking =>
            {
                activityBooking.Questions.ForEach(question =>
                {
                    questions.Add(GetQuestionsFromResponse(question, Constant.ActivityQuestion));
                });

                activityBooking.Passengers.ForEach(passenger =>
                {
                    passenger.PassengerDetails.ForEach(question =>
                    {
                        questions.Add(GetQuestionsFromResponse(question, Constant.PassengerDetails));
                    });

                    passenger.Questions.ForEach(question =>
                    {
                        questions.Add(GetQuestionsFromResponse(question, Constant.PassengerQuestion));
                    });
                });
            });

            return questions;
        }

        private BokunEntities.Question GetQuestionsFromResponse(MainContactDetail questions, string questionType)
        {
            var question = new BokunEntities.Question
            {
                QuestionId = questions.QuestionId,
                OriginalQuestion = questions.OriginalQuestion,
                Pattern = questions.Pattern,
                Label = questions.Label,
                Help = questions.Help,
                Placeholder = questions.Placeholder,
                QuestionCode = questions.QuestionCode,
                SelectFromOptions = questions.SelectFromOptions,
                SelectMultiple = questions.SelectMultiple,
                Required = questions.Required,
                Answers = questions.Answers,
                DataFormat = questions.DataFormat,
                DataType = questions.DataType,
                Flags = questions.Flags,
                DefaultValue = questions.DefaultValue,
                QuestionType = questionType
            };
            if (string.Equals(question?.DataType, "BOOLEAN", System.StringComparison.OrdinalIgnoreCase))
            {
                question.SelectFromOptions = true;
                var trueOption = new BokunEntities.AnswerOption
                {
                    Label = "True",
                    Value = "true"
                };
                var falseOption = new BokunEntities.AnswerOption
                {
                    Label = "False",
                    Value = "false"
                };

                question.AnswerOptions.Add(trueOption);
                question.AnswerOptions.Add(falseOption);
            }
            question.AnswerOptions = new List<BokunEntities.AnswerOption>();
            if (questions.AnswerOptions != null)
            {
                foreach (var item in questions.AnswerOptions)
                {
                    var answerOption = new BokunEntities.AnswerOption()
                    {
                        Label = item.Label,
                        Value = item.Value
                    };
                    question.AnswerOptions.Add(answerOption);
                }
            }
            return question;
        }
    }
}