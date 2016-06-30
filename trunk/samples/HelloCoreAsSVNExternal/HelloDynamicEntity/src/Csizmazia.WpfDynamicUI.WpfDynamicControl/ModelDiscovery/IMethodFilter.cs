using System;
using System.Reflection;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery
{
    public interface IMethodFilter
    {
        Func<MethodInfo, bool> GetFilter();
    }

    public interface IMethodFilter<in TParameter>
    {
        Func<MethodInfo, bool> GetFilter(TParameter parameter);
    }
}