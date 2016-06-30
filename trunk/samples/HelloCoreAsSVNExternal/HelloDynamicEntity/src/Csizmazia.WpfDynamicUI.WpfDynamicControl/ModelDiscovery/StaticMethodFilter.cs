using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Csizmazia.Discovering;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery
{
    public class StaticMethodFilter<TModel> : MethodFilter<TModel> where TModel : class
    {

        public static readonly StaticMethodFilter<TModel> Instance = new StaticMethodFilter<TModel>();


        private StaticMethodFilter()
        {
        }

        public override Func<MethodInfo, bool> GetFilter(TModel parameter)
        {
            return mi => mi.ReturnType == typeof(void) &&
                         !mi.IsAbstract &&
                         !mi.IsConstructor &&
                         mi.GetMethodParameters().Length < 2 &&
                         (mi.GetMethodParameters().Length == 0 ||
                         parameter.GetType().IsAssignableFrom(mi.GetMethodParameters()[0].ParameterType)
                          ) &&
                         (!mi.HasAttribute<DisplayAttribute>() || mi.GetAttribute<DisplayAttribute>().AutoGenerateField);
        }
    }
}