using System;
using System.Collections.Generic;
using Sales.Core.Bases;

namespace Sales.Infrastructure.UsefulModels.Pagination
{
    public abstract class PropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> MappingDictionary { get; }

        protected PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = mappingDictionary;
            MappingDictionary[nameof(EntityBase.Id)] = new PropertyMappingValue(new List<string> { nameof(EntityBase.Id) });
            MappingDictionary[nameof(EntityBase.Order)] = new PropertyMappingValue(new List<string> { nameof(EntityBase.Order) });
            MappingDictionary[nameof(EntityBase.Deleted)] = new PropertyMappingValue(new List<string> { nameof(EntityBase.Deleted) });
        }

        public bool ValidMappingExistsFor(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();
                var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);
                if (!MappingDictionary.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}