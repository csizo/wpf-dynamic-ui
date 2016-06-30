using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Csizmazia.Collections;
using Csizmazia.Discovering;
using Csizmazia.WpfDynamicUI.Localization;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.MethodControlProviders;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders;
using Image = System.Drawing.Image;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl
{
    public interface IControlProviderHandler<in TParameter>
    {
        bool CanProvideControl(TParameter parameter);
        Control ProvideControl(TParameter parameter, object viewModel);
    }

    public class SimplePropertyControlProviderHandler : IControlProviderHandler<PropertyInfo>
    {
        private static readonly Dictionary<Type, PropertyControlProvider> ProviderCache =
            new Dictionary<Type, PropertyControlProvider>
                {
                    {typeof (String), new StringControlProvider()},
                    {typeof (Int32), new IntegerControlProvider()},
                    {typeof (Int32?), new IntegerControlProvider()},
                    {typeof (Decimal), new DecimalControlProvider()},
                    {typeof (Decimal?), new DecimalControlProvider()},
                    {typeof (Double), new DoubleControlProvider()},
                    {typeof (Double?), new DoubleControlProvider()},
                    {typeof (TimeSpan), new TimeSpanControlProvider()},
                    {typeof (TimeSpan?), new TimeSpanControlProvider()},
                    {typeof (DateTime), new DateTimeControlProvider()},
                    {typeof (DateTime?), new DateTimeControlProvider()},
                    {typeof (Boolean), new BooleanControlProvider()},
                    {typeof (Boolean?), new BooleanControlProvider()},
                    {typeof (Image), new ImageControlProvider()},
                };

        #region IControlProviderHandler<PropertyInfo> Members

        public bool CanProvideControl(PropertyInfo parameter)
        {
            return ProviderCache.ContainsKey(parameter.PropertyType);
        }

        public Control ProvideControl(PropertyInfo parameter, object viewModel)
        {
            return ProviderCache[parameter.PropertyType].ProvideControl(parameter, viewModel);
        }

        #endregion
    }

    public class InheritedPropertyControlProviderHandler : IControlProviderHandler<PropertyInfo>
    {
        private static readonly Dictionary<Type, PropertyControlProvider> ProviderCache =
            new Dictionary<Type, PropertyControlProvider>
                {
                    {typeof (Control), new WpfControlControlProvider()},
                    {typeof (IPagedQueryable), new PagedQueryableControlProvider()},
                    //{typeof(IEnumerable<IGpsPoint>),new GpsMapControlProvider()},
                    {typeof (IEnumerable), new ItemsControlProvider()},
                    {typeof (INotifyPropertyChanged), new ModelControlProvider()},
                };

        #region IControlProviderHandler<PropertyInfo> Members

        public bool CanProvideControl(PropertyInfo propertyInfo)
        {
            return ProviderCache.Any(kv => kv.Key.IsAssignableFrom(propertyInfo.PropertyType));
        }

        public Control ProvideControl(PropertyInfo propertyInfo, object viewModel)
        {
            PropertyControlProvider controlProvider =
                ProviderCache.Where(kv => kv.Key.IsAssignableFrom(propertyInfo.PropertyType)).Select(kv => kv.Value).
                    First();
            return controlProvider.ProvideControl(propertyInfo, viewModel);
        }

        #endregion
    }

    public class InstanceMethodControlProviderHandler : IControlProviderHandler<MethodInfo>
    {
        private static readonly InstanceMethodControlProvider InstanceMethodControlProvider =
            new InstanceMethodControlProvider();

        #region IControlProviderHandler<MethodInfo> Members

        public bool CanProvideControl(MethodInfo methodInfo)
        {
            return !methodInfo.IsStatic;
        }

        public Control ProvideControl(MethodInfo methodInfo, object viewModel)
        {
            return InstanceMethodControlProvider.ProvideControl(methodInfo, viewModel);
        }

        #endregion
    }

    public class StaticMethodControlProviderHandler : IControlProviderHandler<MethodInfo>
    {
        private static readonly StaticMethodControlProvider StaticMethodControlProvider =
            new StaticMethodControlProvider();

        #region IControlProviderHandler<MethodInfo> Members

        public bool CanProvideControl(MethodInfo methodInfo)
        {
            return methodInfo.IsStatic;
        }

        public Control ProvideControl(MethodInfo methodInfo, object viewModel)
        {
            return StaticMethodControlProvider.ProvideControl(methodInfo, viewModel);
        }

        #endregion
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
                double width = double.NaN;
                double height = double.NaN;
                double maxWidth = double.NaN;
                double maxHeight = double.NaN;

                if (uiHint.ControlParameters.ContainsKey("Width"))
                    control.SetValue(FrameworkElement.WidthProperty, uiHint.ControlParameters["Width"]);
                if (uiHint.ControlParameters.ContainsKey("Height"))
                    control.SetValue(FrameworkElement.HeightProperty, uiHint.ControlParameters["Height"]);
                if (uiHint.ControlParameters.ContainsKey("MaxWidth"))
                    control.SetValue(FrameworkElement.MaxWidthProperty, uiHint.ControlParameters["MaxWidth"]);
                if (uiHint.ControlParameters.ContainsKey("MaxHeight"))
                    control.SetValue(FrameworkElement.MaxWidthProperty, uiHint.ControlParameters["MaxHeight"]);
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

        //public static Control GetInstanceActionControl(this MethodInfo methodInfo, object viewModel)
        //{
        //    return InstanceMethodControlProvider.ProvideControl(methodInfo, viewModel);
        //}


        //public static Control GetStaticActionControl(this MethodInfo methodInfo, object viewModel)
        //{
        //    return StaticMethodControlProvider.ProvideControl(methodInfo, viewModel);
        //}

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

            public class NonLocalizedText
            {
                public NonLocalizedText(string value)
                {
                    Value = value;
                }

                public string Value { get; set; }
            }

            #endregion
        }

        #endregion
    }

    internal static class ButtonStyler
    {
        private static readonly Dictionary<string, Style> ButtonStyleCache = new Dictionary<string, Style>();

        private static Style GetButtonStyle(string resourceKey)
        {
            return ButtonStyleCache.TryGetValue(resourceKey,
                                                () => Application.Current.TryFindResource(resourceKey) as Style);
        }

        private static List<string> TokenizeName(string text)
        {
            var tokens = new List<string>();

            var builder = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                if (Char.IsUpper(c) && builder.Length > 0)
                {
                    tokens.Add(builder.ToString());
                    builder.Clear();
                }
                builder.Append(c);
            }

            if (builder.Length > 0)
                tokens.Add(builder.ToString());

            return tokens;
        }

        private static bool TrySetButtonStyle(Button button, IEnumerable<string> nameTokens)
        {
            foreach (string nameToken in nameTokens)
            {
                string styleName = string.Format("Button{0}Style", nameToken);
                Style style = GetButtonStyle(styleName);
                if (style != null)
                {
                    button.SetValue(FrameworkElement.StyleProperty, style);
                    return true;
                }
            }

            return false;
        }

        public static void TrySetButtonStyle(this Button button, MethodInfo actionMethod)
        {
            List<string> nameTokens = TokenizeName(actionMethod.Name);
            //add default button style
            nameTokens.Add("");

            TrySetButtonStyle(button, nameTokens);
        }
    }
}