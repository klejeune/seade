using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Seade.Core
{
    public class DefaultValueProvider : IDefaultValueProvider
    {
        public virtual object GetDefaultValue(PropertyInfo propertyInfo)
        {
            object value = null;

            if (propertyInfo.PropertyType == typeof(string))
            {
                value = "";
            }
            else if (propertyInfo.PropertyType.IsArray)
            {
                value = Array.CreateInstance(
                    propertyInfo.PropertyType.GetElementType(),
                    new int[propertyInfo.PropertyType.GetArrayRank()]);
            }
            else if (propertyInfo.PropertyType.GetTypeInfo().IsClass && propertyInfo.PropertyType.GetTypeInfo().DeclaredConstructors.Any(c => !c.GetParameters().Any()))
            {
                value = Activator.CreateInstance(propertyInfo.PropertyType);
            }
            else if (propertyInfo.PropertyType.IsConstructedGenericType &&
                     propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                value = Array.CreateInstance(propertyInfo.PropertyType.GenericTypeArguments.Single(), 0);
            }
            else if (propertyInfo.PropertyType.IsConstructedGenericType &&
                     propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                value =
                    Activator.CreateInstance(
                        typeof(Dictionary<,>).MakeGenericType(propertyInfo.PropertyType.GenericTypeArguments));
            }

            return value;
        }
    }
}