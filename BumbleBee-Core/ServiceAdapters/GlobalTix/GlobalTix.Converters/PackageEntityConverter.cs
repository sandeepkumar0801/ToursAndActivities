using Isango.Entities.Enums;
using Isango.Entities.GlobalTix;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Converters.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;

namespace ServiceAdapters.GlobalTix.GlobalTix.Converters
{
    internal class PackageEntityConverter : ConverterBase, IPackageEntityConverter
	{
		public override object Convert(object objectResult)
		{
			Package pkgData = objectResult as Package;
			if (pkgData == null)
			{
				return null;
			}

			return new GlobalTixPackage()
			{
				Id = pkgData.Id,
				Name = pkgData.Name,
				PaxType = ConvertPassengerType(pkgData.Variation),
				LinkId = pkgData.LinkId,
				CurrencyCode = pkgData.CurrencyCode,
				Price = pkgData.Price
			};
		}

		public override object Convert(object objectResult, object input)
		{
			return Convert(objectResult);
		}

		#region Private Methods
		private PassengerType ConvertPassengerType(EnumValue paxVariation)
		{
			return (Constant.Mapper_PassengerType.TryGetValue(paxVariation.Name, out PassengerType psgrType)) ? psgrType : PassengerType.Adult;
		}
		#endregion Private Methods
	}
}
