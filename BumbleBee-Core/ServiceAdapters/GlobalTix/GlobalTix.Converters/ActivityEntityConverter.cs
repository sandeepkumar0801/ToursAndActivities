using Isango.Entities.Enums;
using Isango.Entities.GlobalTix;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Converters.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;

namespace ServiceAdapters.GlobalTix.GlobalTix.Converters
{
    internal class ActivityEntityConverter : ConverterBase, IActivityEntityConverter
	{
		public override object Convert(object objectResult)
		{
			ActivityInfoData activityInfoData = objectResult as ActivityInfoData;
			if (activityInfoData  == null)
			{
				return null;
			}

			return new GlobalTixActivity()
			{
				Id = activityInfoData.Id,
				Country = ConvertIdentifierWithName(activityInfoData.Country),
				Code = activityInfoData.Code,
				City = ConvertIdentifierWithName(activityInfoData.City),
				ImagePath = activityInfoData.ImagePath,
				Desc = activityInfoData.Desc,
				OpHours = activityInfoData.OpHours,
				Title = activityInfoData.Title,
				TicketTypeGroups = activityInfoData.TicketTypeGroups.ConvertAll(x => ConvertTicketTypeGroup(x)),
				Types = activityInfoData.Types.ConvertAll(x => ConvertIdentifierWithName(x)),
				TicketTypes = activityInfoData.TicketTypes.ConvertAll(x => ConvertTicketType(x)),
                Latitude = activityInfoData.Latitude,
                Longitude = activityInfoData.Longitude
			};
		}

		public override object Convert(object objectResult, object input)
		{
			return Convert(objectResult);
		}

		#region Private Methods
		private Isango.Entities.GlobalTix.GlobalTixIdentifierWithName ConvertIdentifierWithName(Entities.RequestResponseModels.IdentifierWithName idWithName)
		{
			return 
				new Isango.Entities.GlobalTix.GlobalTixIdentifierWithName()
					{
						Id = idWithName.Id,
						Name = idWithName.Name
					};
		}

		private PassengerType ConvertPassengerType(EnumValue paxVariation, bool isCustomName = false)
		{
            if(isCustomName)
            {
                paxVariation.Name = paxVariation.CustomName.ToUpper();
            }
			return (Constant.Mapper_PassengerType.TryGetValue(paxVariation.Name, out PassengerType psgrType)) ? psgrType : PassengerType.Adult;
		}

		private Isango.Entities.GlobalTix.GlobalTixTicketType ConvertTicketType(Entities.RequestResponseModels.TicketType tktType)
		{
			return
				new Isango.Entities.GlobalTix.GlobalTixTicketType()
				{
					Id = tktType.Id,
					Name = tktType.Name,
					CurrencyCode = tktType.CurrencyCode,
					Price = tktType.Price,
					PaxType = ConvertPassengerType(tktType.Variation, !string.IsNullOrEmpty(tktType?.Variation?.CustomName))
				};

		}

		private Isango.Entities.GlobalTix.GlobalTixTicketTypeGroup ConvertTicketTypeGroup(Entities.RequestResponseModels.TicketTypeGroup tktTypeGrp)
		{
			return
				new Isango.Entities.GlobalTix.GlobalTixTicketTypeGroup()
				{
					Id = tktTypeGrp.Id,
					Name = tktTypeGrp.Name,
					Desc = tktTypeGrp.Desc,
					ApplyCapacity = tktTypeGrp.ApplyCapacity,
					Products = tktTypeGrp.Products.ConvertAll(x => new Isango.Entities.GlobalTix.GlobalTixIdentifier() { Id = x.Id })
				};
		}
		#endregion Private Methods
	}
}
