using Isango.Entities.Rezdy;
using ServiceAdapters.Rezdy.Rezdy.Converters.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities.PickUpLocation;
using Util;

namespace ServiceAdapters.Rezdy.Rezdy.Converters
{
    public class GetPickUpLocationConverter : ConverterBase, IGetPickUpLocationConverter
    {
        public override object Convert(string response)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<PickUpLocationResponse>(response.ToString());
            if (result == null) return null;
            return LoadPickUpLocation(result);
        }

        private List<RezdyPickUpLocation> LoadPickUpLocation(PickUpLocationResponse result)
        {
            var pickUpLocations = new List<RezdyPickUpLocation>();

            foreach (var pickUpLocation in result.PickUps.PickUpLocations)
            {
                pickUpLocations.Add(
                    new RezdyPickUpLocation
                    {
                        Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                        LocationName = pickUpLocation.LocationName,
                        AdditionalInstructions = pickUpLocation.AdditionalInstructions,
                        Address = pickUpLocation.Address,
                        Latitude = pickUpLocation.Latitude,
                        Longitude = pickUpLocation.Longitude,
                        MinutesPrior = pickUpLocation.MinutesPrior
                    }
                );
            }

            return pickUpLocations;
        }
    }
}
