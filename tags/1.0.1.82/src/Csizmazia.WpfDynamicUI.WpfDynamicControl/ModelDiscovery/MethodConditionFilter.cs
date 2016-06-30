using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery
{
    public class MethodConditionFilter : PropertyFilter<MethodInfo>
    {
        private static readonly string[] GetMethodConditionPropertyNamesFormat = new[]
                                                                                     {
                                                                                         "Can{0}",
                                                                                         "{0}Condition",
                                                                                         "{0}IsEnabled",
                                                                                         "{0}Enabled"
                                                                                     };

        public static readonly MethodConditionFilter Instance = new MethodConditionFilter();

        private MethodConditionFilter()
        {
        }

        public static IEnumerable<string> GetMethodConditionPropertyNames(MethodInfo methodInfo)
        {
            return GetMethodConditionPropertyNamesFormat.Select(s => string.Format(s, methodInfo.Name));
        }


        public override Func<PropertyInfo, bool> GetFilter(MethodInfo methodInfo)
        {
            return
                pi =>
                pi.PropertyType == typeof (bool) && GetMethodConditionPropertyNames(methodInfo).Contains(pi.Name);
        }
    }
}