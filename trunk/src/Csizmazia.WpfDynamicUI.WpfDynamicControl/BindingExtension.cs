using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Csizmazia.Discovering;
using Csizmazia.WpfDynamicUI.Localization;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl
{

    public static class BindingExtensions
    {
        public static string GetDisplayBindingPath(this PropertyInfo propertyInfo)
        {
            var propertyInfos = propertyInfo.GetInstanceProperties();

            var name = string.Empty;
            if (propertyInfos.Any(pi => pi.Name == name))
                name = "Name";
            else if (propertyInfos.Any(pi => pi.Name.Contains("Name")))
            {
                name = propertyInfos.First(pi => pi.Name.Contains("Name")).Name;
            }
            else if (propertyInfos.Any(pi => pi.Name.Contains("Description")))
            {
                name = propertyInfos.First(pi => pi.Name.Contains("Description")).Name;
            }
            else if (propertyInfos.Any(pi => pi.Name.Contains("Id")))
            {
                name = propertyInfos.First(pi => pi.Name.Contains("Id")).Name;
            }
            else if (propertyInfos.Any(pi => pi.Name.Contains("ID")))
            {
                name = propertyInfos.First(pi => pi.Name.Contains("ID")).Name;
            }
            return name;
        }
    }

    public static class BindingExtension
    {
        private static readonly IControlProviderHandler<PropertyInfo>[] PropertyControlProviderHandlers =
            new IControlProviderHandler<PropertyInfo>[]
                {
                    new SimplePropertyControlProviderHandler(),
                    new InheritedPropertyControlProviderHandler(),
                };

        private static readonly IControlProviderHandler<MethodInfo>[] MethodControlProviderHandlers =
            new IControlProviderHandler<MethodInfo>[]
                {
                    new InstanceMethodControlProviderHandler(),
                    new StaticMethodControlProviderHandler(),
                };

        public static Control GetLabel(this PropertyInfo propertyInfo)
        {
            var label = new Label();

            //string name = propertyInfo.GetDisplayName();
            //label.SetValue(ContentControl.ContentProperty, name);
            label.SetBinding(ContentControl.ContentProperty, propertyInfo.GetDisplayNameBinding());


            var isRequiredAttribute = propertyInfo.GetAttribute<RequiredAttribute>();

            if (isRequiredAttribute != null)
                label.SetValue(Control.FontWeightProperty, FontWeights.Bold);

            return label;
        }

        public static Control GetControl(this PropertyInfo propertyInfo, object viewModel)
        {
            Control control = (from propertyControlProviderHandler in PropertyControlProviderHandlers
                               where propertyControlProviderHandler.CanProvideControl(propertyInfo)
                               select propertyControlProviderHandler.ProvideControl(propertyInfo, viewModel)).
                FirstOrDefault();


            if (control == null)
            {
                control = new ContentControl();
                control.SetBinding(ContentControl.ContentProperty, propertyInfo.GetBinding());
            }


            //string description = propertyInfo.GetDisplayDescription();
            //if (description != null)
            //    control.SetValue(FrameworkElement.ToolTipProperty, description);
            control.SetBinding(FrameworkElement.ToolTipProperty, propertyInfo.GetDisplayDescriptionBinding());

            //set width, height,maxwidht and maxheight properties (if applicable)
            var uiHint = propertyInfo.GetAttribute<UIHintAttribute>();
            if (uiHint != null)
            {
                //double width = double.NaN;
                //double height = double.NaN;
                //double maxWidth = double.NaN;
                //double maxHeight = double.NaN;

                if (uiHint.ControlParameters.ContainsKey(UIHints.DisplayParameters.Width))
                    control.SetValue(FrameworkElement.WidthProperty, uiHint.ControlParameters[UIHints.DisplayParameters.Width]);
                if (uiHint.ControlParameters.ContainsKey(UIHints.DisplayParameters.Height))
                    control.SetValue(FrameworkElement.HeightProperty, uiHint.ControlParameters[UIHints.DisplayParameters.Height]);
                if (uiHint.ControlParameters.ContainsKey(UIHints.DisplayParameters.MaxWidth))
                    control.SetValue(FrameworkElement.MaxWidthProperty, uiHint.ControlParameters[UIHints.DisplayParameters.MaxWidth]);
                if (uiHint.ControlParameters.ContainsKey(UIHints.DisplayParameters.MaxHeight))
                    control.SetValue(FrameworkElement.MaxWidthProperty, uiHint.ControlParameters[UIHints.DisplayParameters.MaxHeight]);
                if (uiHint.ControlParameters.ContainsKey(UIHints.DisplayParameters.MinWidth))
                    control.SetValue(FrameworkElement.MinWidthProperty, uiHint.ControlParameters[UIHints.DisplayParameters.MinWidth]);
                if (uiHint.ControlParameters.ContainsKey(UIHints.DisplayParameters.MinHeight))
                    control.SetValue(FrameworkElement.MinWidthProperty, uiHint.ControlParameters[UIHints.DisplayParameters.MinHeight]);
            }

            return control;
        }


        public static Control GetControl(this MethodInfo methodInfo, object viewModel)
        {
            Control control = (from methodControlProviderHandler in MethodControlProviderHandlers
                               where methodControlProviderHandler.CanProvideControl(methodInfo)
                               select methodControlProviderHandler.ProvideControl(methodInfo, viewModel))
                .FirstOrDefault();

            return control;
        }


        public static Binding GetBinding(this PropertyInfo propertyInfo)
        {
            var binding = new Binding(propertyInfo.Name);

            if (propertyInfo.GetGetMethod(false) != null && propertyInfo.GetSetMethod(false) != null)
            {
                binding.Mode = BindingMode.TwoWay;
            }
            else if (propertyInfo.GetGetMethod(false) != null)
                binding.Mode = BindingMode.OneWay;
            else if (propertyInfo.GetGetMethod(false) != null)
            {
                binding.Mode = BindingMode.OneWayToSource;
            }

            binding.ValidatesOnDataErrors = true;

            return binding;
        }

        #region LocalizationMarkupExtensions

        public static Binding GetDisplayNameBinding(this PropertyInfo propertyInfo)
        {
            var displayAttribute = propertyInfo.GetAttribute<DisplayAttribute>();
            string resourceKey = string.Format("{0}{1}", propertyInfo.Name, "Name");
            Type resourceType = displayAttribute != null ? displayAttribute.ResourceType : null;


            string defaultName = propertyInfo.Name;
            if (displayAttribute != null)
                defaultName = displayAttribute.GetName() ?? defaultName;

            return new DisplayBindingBuilder(resourceType, resourceKey, defaultName).ProvideBinding();
        }

        public static Binding GetDisplayDescriptionBinding(this PropertyInfo propertyInfo)
        {
            var displayAttribute = propertyInfo.GetAttribute<DisplayAttribute>();
            string resourceKey = string.Format("{0}{1}", propertyInfo.Name, "Description");
            Type resourceType = displayAttribute != null ? displayAttribute.ResourceType : null;

            string defaultName = propertyInfo.Name;
            if (displayAttribute != null)
                defaultName = displayAttribute.GetDescription() ?? defaultName;

            return new DisplayBindingBuilder(resourceType, resourceKey, defaultName).ProvideBinding();
        }

        public static Binding GetDisplayPromptBinding(this PropertyInfo propertyInfo)
        {
            var displayAttribute = propertyInfo.GetAttribute<DisplayAttribute>();
            string resourceKey = string.Format("{0}{1}", propertyInfo.Name, "Prompt");
            Type resourceType = displayAttribute != null ? displayAttribute.ResourceType : null;

            string defaultName = "";
            if (displayAttribute != null)
                defaultName = displayAttribute.GetPrompt() ?? defaultName;

            return new DisplayBindingBuilder(resourceType, resourceKey, defaultName).ProvideBinding();
        }

        public static Binding GetDisplayShortNameBinding(this PropertyInfo propertyInfo)
        {
            var displayAttribute = propertyInfo.GetAttribute<DisplayAttribute>();
            string resourceKey = string.Format("{0}{1}", propertyInfo.Name, "ShortName");
            Type resourceType = displayAttribute != null ? displayAttribute.ResourceType : null;

            string defaultName = propertyInfo.Name;
            if (displayAttribute != null)
                defaultName = displayAttribute.GetShortName() ?? defaultName;

            return new DisplayBindingBuilder(resourceType, resourceKey, defaultName).ProvideBinding();
        }


        public static Binding GetDisplayNameBinding(this MethodInfo methodInfo)
        {
            var displayAttribute = methodInfo.GetAttribute<DisplayAttribute>();
            string resourceKey = string.Format("{0}{1}", methodInfo.Name, "Name");
            Type resourceType = displayAttribute != null ? displayAttribute.ResourceType : null;

            string defaultName = methodInfo.Name;
            if (displayAttribute != null)
                defaultName = displayAttribute.GetName() ?? defaultName;

            return new DisplayBindingBuilder(resourceType, resourceKey, defaultName).ProvideBinding();
        }

        public static Binding GetDisplayDescriptionBinding(this MethodInfo methodInfo)
        {
            var displayAttribute = methodInfo.GetAttribute<DisplayAttribute>();
            string resourceKey = string.Format("{0}{1}", methodInfo.Name, "Description");
            Type resourceType = displayAttribute != null ? displayAttribute.ResourceType : null;


            string defaultName = methodInfo.Name;
            if (displayAttribute != null)
                defaultName = displayAttribute.GetDescription() ?? defaultName;

            return new DisplayBindingBuilder(resourceType, resourceKey, defaultName).ProvideBinding();
        }

        public static Binding GetDisplayShortNameBinding(this MethodInfo methodInfo)
        {
            var displayAttribute = methodInfo.GetAttribute<DisplayAttribute>();
            string resourceKey = string.Format("{0}{1}", methodInfo.Name, "ShortName");
            Type resourceType = displayAttribute != null ? displayAttribute.ResourceType : null;


            string defaultName = methodInfo.Name;
            if (displayAttribute != null)
                defaultName = displayAttribute.GetShortName() ?? defaultName;

            return new DisplayBindingBuilder(resourceType, resourceKey, defaultName).ProvideBinding();
        }

        private class DisplayBindingBuilder
        {
            private readonly string _defaultValue;

            private readonly string _resourceKey;
            private readonly Type _resourceType;

            #region Construction

            /// <summary>
            /// Initializes a new instance of the <see cref="DisplayBindingBuilder"/> class.
            /// </summary>
            /// <param name="resourceType"> </param>
            /// <param name="resourceKey">The resourceKey.</param>
            public DisplayBindingBuilder(Type resourceType, string resourceKey, string defaultValue)
            {
                _resourceType = resourceType;
                _resourceKey = resourceKey;
                _defaultValue = defaultValue;
            }

            #endregion

            public Binding ProvideBinding()
            {
                var binding = new Binding("Value");

                if (_resourceType != null && _resourceKey != null)
                {
                    binding.Source = new LocalizedString(_resourceType, _resourceKey, _defaultValue);
                }
                else
                {
                    binding.Source = new NonLocalizedText(_defaultValue);
                }

                return binding;
            }

            #region Nested type: NonLocalizedText

            // ReSharper disable MemberCanBePrivate.Local
            public class NonLocalizedText
            // ReSharper restore MemberCanBePrivate.Local
            {
                public NonLocalizedText(string value)
                {
                    Value = value;
                }

                // ReSharper disable MemberCanBePrivate.Local
                public string Value { get; set; }
                // ReSharper restore MemberCanBePrivate.Local
            }

            #endregion
        }

        #endregion
    }
}