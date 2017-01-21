using System;
using System.Reflection;

namespace Seade
{
    public interface IDefaultValueProvider
    {
        object GetDefaultValue(PropertyInfo propertyInfo);
    }
}