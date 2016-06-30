using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Csizmazia.Discovering;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery
{
    public class InstanceFilterPropertiesFilter : PropertyFilter
    {
        private static readonly Func<PropertyInfo, bool> Filter =
            pi =>
            pi.HasAttribute<DisplayAttribute>() &&
            pi.GetAttribute<DisplayAttribute>().GetAutoGenerateFilter() == true;

        public static readonly InstanceFilterPropertiesFilter Instance = new InstanceFilterPropertiesFilter();

        private InstanceFilterPropertiesFilter()
        {
        }

        public override Func<PropertyInfo, bool> GetFilter()
        {
            return Filter;
        }
    }

    public class InstancePropertiesFilter : PropertyFilter
    {
        private static readonly Func<PropertyInfo, bool> Filter =
            pi =>
            !pi.HasAttribute<DisplayAttribute>()
            ||
            (pi.GetAttribute<DisplayAttribute>().GetAutoGenerateField() == true &&
             pi.GetAttribute<DisplayAttribute>().GetAutoGenerateFilter() != true);

        public static readonly InstancePropertiesFilter Instance = new InstancePropertiesFilter();

        private InstancePropertiesFilter()
        {
        }

        public override Func<PropertyInfo, bool> GetFilter()
        {
            return Filter;
        }
    }
}