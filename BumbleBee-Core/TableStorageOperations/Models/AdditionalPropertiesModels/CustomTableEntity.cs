using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using Constant = TableStorageOperations.Constants.Constant;

namespace TableStorageOperations.Models.AdditionalPropertiesModels
{
    public class CustomTableEntity : TableEntity
    {
        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var entityProperties = base.WriteEntity(operationContext);
            var objectProperties = GetType().GetProperties();

            foreach (var item in objectProperties.Where(f => f.PropertyType == typeof(decimal)))
            {
                entityProperties.Add(Constant.DecimalPrefix + item.Name, new EntityProperty(item.GetValue(this, null).ToString()));
            }

            return entityProperties;
        }
    }
}