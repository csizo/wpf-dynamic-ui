using System;
using System.Reflection;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery
{
    public abstract class PropertyFilter<TParameter> : IPropertyFilter<TParameter>
    {
        #region IPropertyFilter<TParameter> Members

        public abstract Func<PropertyInfo, bool> GetFilter(TParameter parameter);

        #endregion
    }

    public abstract class PropertyFilter : IPropertyFilter
    {
        #region IPropertyFilter Members

        public abstract Func<PropertyInfo, bool> GetFilter();

        #endregion
    }
}