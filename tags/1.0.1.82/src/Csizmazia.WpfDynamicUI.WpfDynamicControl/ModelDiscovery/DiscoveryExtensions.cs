using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Csizmazia.Discovering;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery
{
    public static class DiscoveryExtensions
    {
        public static PropertyInfo[] GetPropertiesForUserInterface(this Type modelType)
        {
            return
                modelType.GetInstanceProperties()
                    .Where(InstancePropertiesFilter.Instance.GetFilter())
                    .ToArray();
        }

        public static PropertyInfo[] GetFilterPropertiesForUserInterface(this Type modelType)
        {
            return
                modelType.GetInstanceProperties()
                    .Where(InstanceFilterPropertiesFilter.Instance.GetFilter())
                    .ToArray();
        }

        public static bool IsFilterPropertyForUserInterface(this PropertyInfo propertyInfo)
        {
            return InstanceFilterPropertiesFilter.Instance.GetFilter().Invoke(propertyInfo);
        }

        public static PropertyInfo GetPropertyForUserInterfaceActionCondition(this MethodInfo methodInfo)
        {
            return
                methodInfo.DeclaringType.GetInstanceProperties()
                    .Where(MethodConditionFilter.Instance.GetFilter(methodInfo))
                    .SingleOrDefault();
        }

        public static PropertyInfo GetStaticPropertyForUserInterfaceActionCondition(this MethodInfo methodInfo)
        {
            return
                methodInfo.DeclaringType.GetStaticProperties()
                    .Where(MethodConditionFilter.Instance.GetFilter(methodInfo))
                    .SingleOrDefault();
        }

        public static PropertyInfo GetPropertyForSelectedItem(this PropertyInfo propertyInfo)
        {
            if (!typeof (IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                throw new ArgumentException("property must be an IEnumerable to get the SelectedItem property");

            return GetPropertyForSelectedItem(propertyInfo, propertyInfo.DeclaringType);
        }

        public static PropertyInfo GetPropertyForSelectedItem(this PropertyInfo propertyInfo, Type viewModelType)
        {
            if (!typeof (IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                throw new ArgumentException("property must be an IEnumerable to get the SelectedItem property");

            return
                viewModelType.GetInstanceProperties()
                    .SingleOrDefault(
                        pi => SelectedItemPropertyFilter.GetPossiblePropertyNames(propertyInfo).Contains(pi.Name));
        }

        public static PropertyInfo GetPropertyForSelectedItem(this PropertyInfo propertyInfo,
                                                              string ienumerablePropertyName, Type viewModelType)
        {
            if (!typeof (IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                throw new ArgumentException("property must be an IEnumerable to get the SelectedItem property");

            return
                viewModelType.GetInstanceProperties()
                    .SingleOrDefault(
                        pi =>
                        SelectedItemPropertyFilter.GetPossiblePropertyNames(ienumerablePropertyName).Contains(pi.Name));
        }

        public static bool HasPropertyForSelectedItem(this PropertyInfo propertyInfo)
        {
            if (!typeof (IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                throw new ArgumentException("property must be an IEnumerable to get the SelectedItem property");


            return HasPropertyForSelectedItem(propertyInfo, propertyInfo.DeclaringType);
        }

        public static bool HasPropertyForSelectedItem(this PropertyInfo propertyInfo, Type viewModelType)
        {
            if (!typeof (IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                throw new ArgumentException("property must be an IEnumerable to get the SelectedItem property");

            return
                viewModelType.GetInstanceProperties()
                    .Any(pi => SelectedItemPropertyFilter.GetPossiblePropertyNames(propertyInfo).Contains(pi.Name));
        }

        public static bool HasPropertyForSelectedItem(this PropertyInfo propertyInfo, string ienumerablePropertyName,
                                                      Type viewModelType)
        {
            if (!typeof (IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                throw new ArgumentException("property must be an IEnumerable to get the SelectedItem property");

            return
                viewModelType.GetInstanceProperties()
                    .Any(
                        pi =>
                        SelectedItemPropertyFilter.GetPossiblePropertyNames(ienumerablePropertyName).Contains(pi.Name));
        }

        public static MethodInfo[] GetMethodsForUserInterfaceAction(this Type type)
        {
            MethodInfo[] methods = type.GetInstanceMethods()
                .Where(InstanceMethodFilter.Instance.GetFilter())
                .ToArray();
            return methods;
        }

        public static MethodInfo[] GetStaticMethodsForUserInterfaceAction(this Type type, object model)
        {
            MethodInfo[] methods = type.GetStaticMethods()
                .Where(StaticMethodFilter<object>.Instance.GetFilter(model))
                .ToArray();
            return methods;
        }
    }
}