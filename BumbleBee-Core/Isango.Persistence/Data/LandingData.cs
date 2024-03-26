using Isango.Entities;
using System.Data;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.Data
{
    public class LandingData
    {
        /// <summary>
        /// This method returns localized merchandising data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public LocalizedMerchandising LoadLocalizedMerchandisingData(IDataReader reader)
        {
            var localizedMerchandising = new LocalizedMerchandising
            {
                Name = DbPropertyHelper.StringPropertyFromRow(reader, Constant.SourceName),
                Id = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.SourceId),
                Link = DbPropertyHelper.StringPropertyFromRow(reader, Constant.Url),
                Image = DbPropertyHelper.StringPropertyFromRow(reader, Constant.ImageName),
                ImageId = DbPropertyHelper.StringPropertyFromRow(reader, Constant.ImageId),
                SourceMarket = DbPropertyHelper.StringPropertyFromRow(reader, Constant.Locale),
                Type = DbPropertyHelper.StringPropertyFromRow(reader, Constant.TypeId),
                Language = DbPropertyHelper.StringPropertyFromRow(reader, Constant.Language),
                AffiliateId = DbPropertyHelper.StringPropertyFromRow(reader, Constant.ReaderAffiliateId)
            };
            return localizedMerchandising;
        }
    }
}