using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Csizmazia.Discovering;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery
{
    public class InstanceMethodFilter : MethodFilter
    {
        public static readonly InstanceMethodFilter Instance = new InstanceMethodFilter();

        private static readonly Func<MethodInfo, bool> Filter = mi =>
                                                                mi.ReturnType == typeof (void) &&
                                                                !mi.IsAbstract &&
                                                                !mi.IsConstructor &&
                                                                mi.GetMethodParameters().Length == 0 &&
                                                                (!mi.HasAttribute<DisplayAttribute>() ||
                                                                 mi.GetAttribute<DisplayAttribute>().AutoGenerateField);

        private InstanceMethodFilter()
        {
        }

        public override Func<MethodInfo, bool> GetFilter()
        {
            return Filter;
        }
    }
}