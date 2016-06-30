using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Csizmazia.Discovering
{
    public static class ReflectionCache
    {
        private static readonly Dictionary<Type, PropertyInfo[]> InstanceProperties =
            new Dictionary<Type, PropertyInfo[]>();

        private static readonly Dictionary<Type, PropertyInfo[]> StaticProperties =
            new Dictionary<Type, PropertyInfo[]>();

        private static readonly Dictionary<Type, MethodInfo[]> InstanceMethods = new Dictionary<Type, MethodInfo[]>();

        private static readonly Dictionary<Type, MethodInfo[]> StaticMethods = new Dictionary<Type, MethodInfo[]>();

        private static readonly Dictionary<Type, FieldInfo[]> InstanceFields = new Dictionary<Type, FieldInfo[]>();

        private static readonly Dictionary<MethodInfo, ParameterInfo[]> MethodParameters =
            new Dictionary<MethodInfo, ParameterInfo[]>();


        private static readonly Dictionary<MemberInfo, Attribute[]> MemberAttributes =
            new Dictionary<MemberInfo, Attribute[]>();

        private static readonly Dictionary<MemberInfo, Attribute[]> MemberInheritedAttributes =
            new Dictionary<MemberInfo, Attribute[]>();

        private static readonly Type[] CollectionTypes = new[]
                                                             {
                                                                 typeof (Collection<>),
                                                                 typeof (ObservableCollection<>),
                                                                 typeof (List<>)
                                                             };


        private static Dictionary<MemberInfo, Attribute[]> GetAttributeCache(bool inherit)
        {
            if (inherit)
                return MemberInheritedAttributes;

            return MemberAttributes;
        }

        public static ParameterInfo[] GetMethodParameters(this MethodInfo methodInfo)
        {
            return MethodParameters.TryGetValue(methodInfo, methodInfo.GetParameters);
        }

        public static PropertyInfo[] GetInstanceProperties<T>(this T instance)
        {
            return GetInstanceProperties(instance.GetType());
        }

        public static PropertyInfo[] GetInstanceProperties(this Type type)
        {
            return InstanceProperties.TryGetValue(type,
                                                  () => type.GetProperties(BindingFlags.Public | BindingFlags.Instance));
        }


        public static PropertyInfo[] GetStaticProperties<T>(this T instance)
        {
            return GetInstanceProperties(instance.GetType());
        }

        public static PropertyInfo[] GetStaticProperties(this Type type)
        {
            return StaticProperties.TryGetValue(type,
                                                () => type.GetProperties(BindingFlags.Public | BindingFlags.Static));
        }

        public static MethodInfo[] GetInstanceMethods<T>(this T instance)
        {
            return GetInstanceMethods(instance.GetType());
        }

        public static MethodInfo[] GetInstanceMethods(this Type type)
        {
            return InstanceMethods.TryGetValue(type, () => type.GetMethods(BindingFlags.Public | BindingFlags.Instance));
        }

        public static MethodInfo[] GetStaticMethods<T>(this T instance)
        {
            return GetStaticMethods(instance.GetType());
        }

        public static MethodInfo[] GetStaticMethods(this Type type)
        {
            return StaticMethods.TryGetValue(type, () => type.GetMethods(BindingFlags.Public | BindingFlags.Static));
        }

        public static FieldInfo[] GetInstanceFields<T>(this T instance)
        {
            return GetInstanceFields(instance.GetType());
        }

        public static FieldInfo[] GetInstanceFields(this Type type)
        {
            return InstanceFields.TryGetValue(type, () => type.GetFields(BindingFlags.Public | BindingFlags.Instance));
        }

        public static bool HasAttribute<TAttribute>(this MemberInfo memberInfo, bool inherit = false)
            where TAttribute : Attribute
        {
            return GetAttributes<TAttribute>(memberInfo, inherit).Any();
        }

        public static TAttribute GetAttribute<TAttribute>(this MemberInfo memberInfo, bool inherit = false)
            where TAttribute : Attribute
        {
            return GetAttributeCache(inherit).TryGetValue(memberInfo,
                                                          () =>
                                                          memberInfo.GetCustomAttributes(inherit)
                                                              .OfType<Attribute>()
                                                              .ToArray())
                .OfType<TAttribute>()
                .SingleOrDefault();
        }

        public static TAttribute GetAttribute<TAttribute>(this MemberInfo memberInfo, Func<TAttribute> defaultIfEmpty,
                                                          bool inherit = false) where TAttribute : Attribute
        {
            return GetAttributeCache(inherit).TryGetValue(memberInfo,
                                                          () =>
                                                          memberInfo.GetCustomAttributes(inherit)
                                                              .OfType<Attribute>()
                                                              .ToArray())
                .OfType<TAttribute>()
                .DefaultIfEmpty(defaultIfEmpty())
                .SingleOrDefault();
        }

        public static TAttribute[] GetAttributes<TAttribute>(this MemberInfo memberInfo, bool inherit = false)
            where TAttribute : Attribute
        {
            return GetAttributeCache(inherit).TryGetValue(memberInfo,
                                                          () =>
                                                          memberInfo.GetCustomAttributes(inherit)
                                                              .OfType<Attribute>()
                                                              .ToArray())
                .OfType<TAttribute>()
                .ToArray();
        }

        public static Type GetCollectionItemType(this Type type)
        {
            if (!type.IsGenericType)
                throw new ArgumentException();

            Type genericTypeDefinition = type.GetGenericTypeDefinition();

            if (CollectionTypes.Contains(genericTypeDefinition))
            {
                return type.GetGenericArguments().First();
            }


            return GetCollectionItemType(type.BaseType);
        }
    }
}