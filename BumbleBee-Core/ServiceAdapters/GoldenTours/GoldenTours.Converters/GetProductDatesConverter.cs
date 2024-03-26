using Logger.Contract;
using ServiceAdapters.GoldenTours.GoldenTours.Converters.Contracts;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.GetProductDates;

namespace ServiceAdapters.GoldenTours.GoldenTours.Converters
{
    public class GetProductDatesConverter : ConverterBase, IGetProductDatesConverter
    {
        public GetProductDatesConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override object Convert<T>(T response, T request)
        {
            var result = DeSerializeXml<GetProductDatesResponse>(response as string);
            if (result != null)
            {
                var productDatesAvailability = ConvertProductDatesResponse(result);
                return productDatesAvailability;
            }
            return null;
        }

        #region Private Methods

        private List<DateTime> ConvertProductDatesResponse(GetProductDatesResponse response)
        {
            var availableDateTimes = new List<DateTime>();
            var availableDates = response?.Dates?.Monthyear;
            if (availableDates == null) return null;

            foreach (var availableDate in availableDates)
            {
                var value = availableDate?.Value;
                var month = GetMonth(value?.Substring(0, 3));
                var year = System.Convert.ToInt32(value?.Substring(4, value.Length - 4));

                var days = availableDate?.Days?.Day;
                if (days == null) continue;

                foreach (var dayString in days)
                {
                    var day = System.Convert.ToInt32(dayString);
                    var dateTime = new DateTime(year, month, day);
                    availableDateTimes.Add(dateTime);
                }
            }

            return availableDateTimes;
        }

        private int GetMonth(string monthName)
        {
            int month = 0;
            switch (monthName)
            {
                case "Jan":
                    month = 1;
                    break;

                case "Feb":
                    month = 2;
                    break;

                case "Mar":
                    month = 3;
                    break;

                case "Apr":
                    month = 4;
                    break;

                case "May":
                    month = 5;
                    break;

                case "Jun":
                    month = 6;
                    break;

                case "Jul":
                    month = 7;
                    break;

                case "Aug":
                    month = 8;
                    break;

                case "Sep":
                    month = 9;
                    break;

                case "Oct":
                    month = 10;
                    break;

                case "Nov":
                    month = 11;
                    break;

                case "Dec":
                    month = 12;
                    break;
            }
            return month;
        }

        #endregion Private Methods
    }
}