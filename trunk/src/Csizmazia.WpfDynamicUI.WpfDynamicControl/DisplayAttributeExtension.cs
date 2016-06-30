using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using Csizmazia.Discovering;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl
{
    internal static class DisplayAttributeExtension
    {
        private static readonly Dictionary<Type, ResourceManager> ResourceManagerCache =
            new Dictionary<Type, ResourceManager>();

        private static readonly Dictionary<Tuple<Type, CultureInfo, string>, string> ResourceCache =
            new Dictionary<Tuple<Type, CultureInfo, string>, string>();


        public static string GetDisplayName(this PropertyInfo propertyInfo)
        {
            string displayName = propertyInfo.Name;

            var displayAttribute = propertyInfo.GetAttribute<DisplayAttribute>();
            if (displayAttribute == null)
                return displayName;


            if (displayAttribute.ResourceType != null)
            {
                string resourceKey = string.Format("{0}{1}", propertyInfo.Name, "Name");

                displayName = ReadResourceString(displayAttribute.ResourceType, resourceKey, displayName);
            }
            else if (!string.IsNullOrEmpty(displayAttribute.Name))
            {
                displayName = displayAttribute.Name;
            }

            return displayName;
        }

        public static string GetDisplayDescription(this PropertyInfo propertyInfo)
        {
            string displayDescription = propertyInfo.Name;

            var displayAttribute = propertyInfo.GetAttribute<DisplayAttribute>();
            if (displayAttribute == null)
                return displayDescription;


            if (displayAttribute.ResourceType != null)
            {
                string resourceKey = string.Format("{0}{1}", propertyInfo.Name, "Description");

                displayDescription = ReadResourceString(displayAttribute.ResourceType, resourceKey, null);
            }
            else if (!string.IsNullOrEmpty(displayAttribute.GetDescription()))
            {
                displayDescription = displayAttribute.GetDescription();
            }

            return displayDescription;
        }

        public static string GetDisplayPrompt(this PropertyInfo propertyInfo)
        {
            string displayPrompt = propertyInfo.Name;

            var displayAttribute = propertyInfo.GetAttribute<DisplayAttribute>();
            if (displayAttribute == null)
                return displayPrompt;


            if (displayAttribute.ResourceType != null)
            {
                string resourceKey = string.Format("{0}{1}", propertyInfo.Name, "Prompt");

                displayPrompt = ReadResourceString(displayAttribute.ResourceType, resourceKey, null);
            }
            else if (!string.IsNullOrEmpty(displayAttribute.GetPrompt()))
            {
                displayPrompt = displayAttribute.GetPrompt();
            }

            return displayPrompt;
        }

        public static string GetDisplayShortName(this PropertyInfo propertyInfo)
        {
            string displayShortName = propertyInfo.Name;

            var displayAttribute = propertyInfo.GetAttribute<DisplayAttribute>();
            if (displayAttribute == null)
                return displayShortName;


            if (displayAttribute.ResourceType != null)
            {
                string resourceKey = string.Format("{0}{1}", propertyInfo.Name, "ShortName");

                displayShortName = ReadResourceString(displayAttribute.ResourceType, resourceKey, displayShortName);
            }
            else if (!string.IsNullOrEmpty(displayAttribute.GetShortName()))
            {
                displayShortName = displayAttribute.GetShortName();
            }

            return displayShortName;
        }

        public static string GetDisplayName(this MethodInfo methodInfo)
        {
            string displayName = methodInfo.Name;

            var displayAttribute = methodInfo.GetAttribute<DisplayAttribute>();
            if (displayAttribute == null)
                return displayName;


            if (displayAttribute.ResourceType != null)
            {
                string resourceKey = string.Format("{0}{1}", methodInfo.Name, "Name");

                displayName = ReadResourceString(displayAttribute.ResourceType, resourceKey, displayName);
            }
            else if (!string.IsNullOrEmpty(displayAttribute.Name))
            {
                displayName = displayAttribute.Name;
            }

            return displayName;
        }

        public static string GetDisplayDescription(this MethodInfo methodInfo)
        {
            string displayDescription = methodInfo.Name;

            var displayAttribute = methodInfo.GetAttribute<DisplayAttribute>();
            if (displayAttribute == null)
                return displayDescription;


            if (displayAttribute.ResourceType != null)
            {
                string resourceKey = string.Format("{0}{1}", methodInfo.Name, "Description");

                displayDescription = ReadResourceString(displayAttribute.ResourceType, resourceKey, null);
            }
            else if (!string.IsNullOrEmpty(displayAttribute.GetDescription()))
            {
                displayDescription = displayAttribute.GetDescription();
            }

            return displayDescription;
        }

        public static string GetDisplayPrompt(this MethodInfo methodInfo)
        {
            string displayPrompt = methodInfo.Name;

            var displayAttribute = methodInfo.GetAttribute<DisplayAttribute>();
            if (displayAttribute == null)
                return displayPrompt;


            if (displayAttribute.ResourceType != null)
            {
                string resourceKey = string.Format("{0}{1}", methodInfo.Name, "Prompt");

                displayPrompt = ReadResourceString(displayAttribute.ResourceType, resourceKey, null);
            }
            else if (!string.IsNullOrEmpty(displayAttribute.GetPrompt()))
            {
                displayPrompt = displayAttribute.GetPrompt();
            }

            return displayPrompt;
        }

        public static string GetDisplayShortName(this MethodInfo methodInfo)
        {
            string displayShortName = methodInfo.Name;

            var displayAttribute = methodInfo.GetAttribute<DisplayAttribute>();
            if (displayAttribute == null)
                return displayShortName;


            if (displayAttribute.ResourceType != null)
            {
                string resourceKey = string.Format("{0}{1}", methodInfo.Name, "ShortName");

                displayShortName = ReadResourceString(displayAttribute.ResourceType, resourceKey, displayShortName);
            }
            else if (!string.IsNullOrEmpty(displayAttribute.GetShortName()))
            {
                displayShortName = displayAttribute.GetShortName();
            }

            return displayShortName;
        }

        private static string ReadResourceString(Type resourceType, string resourcekey, string defaultText)
        {
            ResourceManager resourceManager = ResourceManagerCache.TryGetValue(resourceType,
                                                                               () => new ResourceManager(resourceType));


            string resourceText =
                ResourceCache.TryGetValue(
                    new Tuple<Type, CultureInfo, string>(resourceType, Thread.CurrentThread.CurrentUICulture,
                                                         resourcekey),
                    () =>
                        {
                            string text = resourceManager.GetString(resourcekey) ?? defaultText;
                            return text;
                        }
                    );

            return resourceText;
        }
    }
}