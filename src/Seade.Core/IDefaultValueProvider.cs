using System;
using System.Reflection;

namespace Seade.Core
{
    public interface IDefaultValueProvider
    {
        object GetDefaultValue(PropertyInfo propertyInfo);
    }
}