using System;
using System.Reflection;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery
{
    public interface IPropertyFilter
    {
        Func<PropertyInfo, bool> GetFilter();
    }

    public interface IPropertyFilter<in TParameter>
    {
        Func<PropertyInfo, bool> GetFilter(TParameter parameter);
    }
}