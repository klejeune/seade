using System;
using System.Reflection;
using Seade.Core;

namespace Seade.Test.Models
{
    public class TestDefaultValueProvider : DefaultValueProvider
    {
        private readonly Func<PropertyInfo, object> builder;

        public TestDefaultValueProvider(Func<PropertyInfo, object> builder)
        {
            this.builder = builder;
        }

        public override object GetDefaultValue(PropertyInfo propertyInfo)
        {
            var value = builder(propertyInfo);

            if (value != null)
            {
                return value;
            }
            else
            {
                return base.GetDefaultValue(propertyInfo);
            }
        }
    }
}
