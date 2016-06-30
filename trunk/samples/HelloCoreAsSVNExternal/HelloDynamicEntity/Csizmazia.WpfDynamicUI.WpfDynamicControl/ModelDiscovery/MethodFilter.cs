using System;
using System.Reflection;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery
{
    public abstract class MethodFilter<TParameter> : IMethodFilter<TParameter>
    {
        #region IMethodFilter<TParameter> Members

        public abstract Func<MethodInfo, bool> GetFilter(TParameter parameter);

        #endregion
    }

    public abstract class MethodFilter : IMethodFilter
    {
        #region IMethodFilter Members

        public abstract Func<MethodInfo, bool> GetFilter();

        #endregion
    }
}