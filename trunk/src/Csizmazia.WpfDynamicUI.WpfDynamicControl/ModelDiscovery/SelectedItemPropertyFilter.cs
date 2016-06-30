using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery
{
    public class SelectedItemPropertyFilter : PropertyFilter<PropertyInfo>
    {
        public static readonly SelectedItemPropertyFilter Instance =
            new SelectedItemPropertyFilter();

        private static readonly string[] PossiblePropertyNames = new[]
                                                                     {
                                                                         "Selected{0}",
                                                                         "Selected{0}Item",
                                                                         "{0}"
                                                                     };

        private SelectedItemPropertyFilter()
        {
        }

        public static IEnumerable<string> GetPossiblePropertyNames(PropertyInfo propertyInfo)
        {
            return GetPossiblePropertyNames(propertyInfo.Name);
        }

        public static IEnumerable<string> GetPossiblePropertyNames(string ienumerablePropertyName)
        {
            var a = PossiblePropertyNames.Select(possiblePropertyName => string.Format(possiblePropertyName, ienumerablePropertyName));


            var unlistedPropertyName = ienumerablePropertyName.Replace("List", "");
            var b =
                PossiblePropertyNames.Select(
                    possiblePropertyName => string.Format(possiblePropertyName, unlistedPropertyName));

            return a.Union(b).Distinct();
        }


        public override Func<PropertyInfo, bool> GetFilter(PropertyInfo parameter)
        {
            return c => GetPossiblePropertyNames(parameter).Contains(parameter.Name);
        }
    }
}