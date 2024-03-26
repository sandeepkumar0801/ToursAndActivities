using Isango.Entities.Activities;
using Logger.Contract;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Converters.Contracts;
using ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels;
using Util;

namespace ServiceAdapters.FareHarbor.FareHarbor.Converters
{
    public class ItemsConverter : ConverterBase, IItemsConverter
    {
        public ItemsConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to isango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult">string response Companies Call</param>
        /// <param name="criteria"></param>
        /// <returns>Isango.Entities.Supplier List Object</returns>
        public override object Convert<T>(T objectResult, object criteria)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<ItemsResponse>(objectResult as string);

            if (result.Items != null)
            {
                var itemsList = ConvertItemsResult(result);
                return itemsList;
            }

            return null;
        }

        /// <summary>
        /// This method maps the API response to isango Contracts objects.
        /// </summary>
        /// <returns> Isango.Entities.Supplier Object</returns>
        private object ConvertItemsResult(ItemsResponse itemsList)
        {
            var activityList = new List<Activity>();
            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
            Parallel.ForEach(itemsList.Items, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, item =>
            {
                var activity = new Activity
                {
                    Name = item.Name,
                    CancellationPolicy = item.CancellationPolicy,
                    AdditionalInfo = item.CancellationPolicySafeHtml,
                    Introduction = item.Description,
                    FactsheetId = item.Pk,
                    IsServiceLevelPickUp = item.IsPickupEverAvailable,
                    DurationString = item.Headline,
                    CustomerPrototypes = new List<Isango.Entities.CustomerPrototype>()
                };
                activity.CustomerPrototypes = GetCustomerPrototypes(item);
                activityList.Add(activity);
            });

            return activityList;
        }

        private List<Isango.Entities.CustomerPrototype> GetCustomerPrototypes(Item item)
        {
            var customerPrototypeList = new List<Isango.Entities.CustomerPrototype>();

            //Adult
            var customerItemPrototype = item.CustomerPrototypes.FirstOrDefault(x => x.DisplayName.Contains(Constant.Adult));
            if (customerItemPrototype != null)
            {
                customerPrototypeList.Add(new Isango.Entities.CustomerPrototype
                {
                    PassengerType = Isango.Entities.Enums.PassengerType.Adult
                });
            }

            //Child
            customerItemPrototype = item.CustomerPrototypes.FirstOrDefault(x => x.DisplayName.Contains(Constant.Child));
            if (customerItemPrototype != null)
            {
                customerPrototypeList.Add(new Isango.Entities.CustomerPrototype
                {
                    PassengerType = Isango.Entities.Enums.PassengerType.Child
                });
            }

            return customerPrototypeList;
        }
    }
}